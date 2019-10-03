using System.Linq;
using System.Collections.Generic;

namespace lineupSim.Models 
{
    public class Team
    {
        private int _index = 0;
        
        private Player[] _players { get; set; }

        public Team(IEnumerable<Player> players)
        {
            _players = players.ToArray();
        }

        public Player NextBatter()
        {
            var batter = _players[_index];

            _index++;
            if (_index >= _players.Length) _index = 0;

            return batter;
        }
    }
}