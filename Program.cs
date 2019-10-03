using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using lineupSim.Models;
using System.Collections.Generic;
using System;
using System.IO;
using System.Threading;

namespace lineupSim
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var gamesPerPerm = 100;

            if (int.TryParse(args[0], out var games))
            {
                gamesPerPerm = games;
            }

            ThreadPool.GetMinThreads(out var minWorker, out var minIOC);
            ThreadPool.SetMinThreads(100000, minIOC);

            ThreadLocal<Random> random = new ThreadLocal<Random>(() => new Random());
            
            var players = ReadFile().OrderByDescending(x => x.PAs);

            var teamPlayers = new List<Player> {
                players.First(x => x.Team == "STL" && x.Position == "1B"),
                players.First(x => x.Team == "STL" && x.Position == "2B"),
                players.First(x => x.Team == "STL" && x.Position == "3B"),
                players.First(x => x.Team == "STL" && x.Position == "SS"),
                players.First(x => x.Team == "STL" && x.Position == "P"),
                players.First(x => x.Team == "STL" && x.Position == "C"),
                players.First(x => x.Team == "STL" && x.Position == "LF"),
                players.First(x => x.Team == "STL" && x.Position == "CF"),
                players.First(x => x.Team == "STL" && x.Position == "RF")
            };
            
            var tasks = new List<Task<(int Score, List<Player> Lineup)>>();

            var sw = new Stopwatch();

            sw.Start();

            foreach (var perm in teamPlayers.GetPermutations(9))
            {
                tasks.Add(Task.Run<(int Score, List<Player> Lineup)>(async () => {
                    var innerTasks = new List<Task<int>>();
                    
                    for (var i = 0; i < gamesPerPerm; i++)
                    {
                        var team = new Team(perm);
                        innerTasks.Add(Task.Run<int>(() => SimGame(team, random.Value)));
                    }    

                    await Task.WhenAll(innerTasks);

                    var permScore = innerTasks.Sum(x => x.Result);

                    return (permScore, perm);
                }));
            }

            await Task.WhenAll(tasks);

            var results = tasks.Select(x => x.Result);

            sw.Stop();

            Console.WriteLine(sw.Elapsed);

            var bestLineups = results.OrderByDescending(x => x.Score).Take(10);

            foreach (var lineup in bestLineups)
            {
                Console.WriteLine(lineup.Score);
                foreach (var player in lineup.Lineup)
                {
                    Console.WriteLine(player.Name);
                }
                Console.WriteLine();
            }
        }

        static int SimGame(Team team, Random random)
        {
            var totalScore = 0;            

            for (var i = 0; i < 9; i++)
            {
                var inningState = InningState.Initial;
                while (!inningState.Completed)
                {
                    var batter = team.NextBatter();
                    var outcome = batter.GetOutcome(random.NextDouble());
                    inningState = inningState.Transition(outcome);
                }
                totalScore += inningState.Score;
            }

            return totalScore;
        }

        static IEnumerable<Player> ReadFile()
        {
            using(var reader = new StreamReader(@"./Data/players.csv"))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    if (values[0] != "RK")
                    {
                        var player = new Player
                        {
                            Name = values[1],
                            Team = values[2],
                            Position = values[3],
                            Singles = int.Parse(values[4]),
                            Doubles = int.Parse(values[5]),
                            Triples = int.Parse(values[6]),
                            HomeRuns = int.Parse(values[7]),
                            Walks = int.Parse(values[8]),
                            StrikeOuts = int.Parse(values[9]),
                            GroundOuts = int.Parse(values[10]),
                            FlyOuts = int.Parse(values[11])
                        };

                        player.Initialize();

                        yield return player;
                    }
                    
                }
            }
        }
    }
}
