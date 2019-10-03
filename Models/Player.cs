using System;

namespace lineupSim.Models 
{
    public class Player
    {
        public string Name { get; set; }
        public string Team { get; set; }
        public string Position { get; set; }

        public int Walks { get; set; }
        public int Singles { get; set; }
        public int Doubles { get; set; }
        public int Triples { get; set; }
        public int HomeRuns { get; set; }
        public int FlyOuts { get; set; }
        public int GroundOuts { get; set; }
        public int StrikeOuts { get; set; }
        
        public int PAs { get; set; }
        public Decimal WalkPct { get; set; }
        public Decimal SinglePct { get; set; }
        public Decimal DoublePct { get; set; }
        public Decimal TriplePct { get; set; }
        public Decimal HomeRunPct { get; set; }
        public Decimal FlyOutPct { get; set; }
        public Decimal GroundOutPct { get; set; }
        public Decimal StrikeOutPct { get; set; }
        public Decimal WalkThreshold { get; set; }
        public Decimal SingleThreshold { get; set; }
        public Decimal DoubleThreshold { get; set; }
        public Decimal TripleThreshold { get; set; }
        public Decimal HomeRunThreshold { get; set; }
        public Decimal FlyOutThreshold { get; set; }
        public Decimal GroundOutThreshold { get; set; }
        public Decimal StrikeOutThreshold { get; set; }

        public void Initialize() 
        {
            PAs = Walks
                + Singles
                + Doubles
                + Triples
                + HomeRuns
                + FlyOuts
                + GroundOuts
                + StrikeOuts;

            WalkPct = SafePct(Walks, PAs);
            WalkThreshold = WalkPct;
            SinglePct = SafePct(Singles, PAs);
            SingleThreshold = WalkThreshold + SinglePct;
            DoublePct = SafePct(Doubles, PAs);
            DoubleThreshold = SingleThreshold + DoublePct;
            TriplePct = SafePct(Triples, PAs);
            TripleThreshold = DoubleThreshold + TriplePct;
            HomeRunPct = SafePct(HomeRuns, PAs);
            HomeRunThreshold = TripleThreshold + HomeRunPct;
            FlyOutPct = SafePct(FlyOuts, PAs);
            FlyOutThreshold = HomeRunThreshold + FlyOutPct;
            GroundOutPct = SafePct(GroundOuts, PAs);
            GroundOutThreshold = FlyOutThreshold + GroundOutPct;
            StrikeOutPct = SafePct(StrikeOuts, PAs);
            StrikeOutThreshold = GroundOutThreshold + StrikeOutPct;
        }

        public PlateAppearanceOutcome GetOutcome(double randomDouble)
        {
            var random = (decimal)randomDouble;

            if (random < 0 || random > 1)
            {
                throw new ArgumentException($"Invalid value for {nameof(random)}");
            }

            switch (random)
            {
                case decimal r when (r < WalkThreshold):
                    return PlateAppearanceOutcome.Walk;
                case decimal r when (r < SingleThreshold):
                    return PlateAppearanceOutcome.Single;
                case decimal r when (r < DoubleThreshold):
                    return PlateAppearanceOutcome.Double;
                case decimal r when (r < TripleThreshold):
                    return PlateAppearanceOutcome.Triple;
                case decimal r when (r < HomeRunThreshold):
                    return PlateAppearanceOutcome.HomeRun;
                case decimal r when (r < FlyOutThreshold):
                    return PlateAppearanceOutcome.FlyOut;
                case decimal r when (r < GroundOutThreshold):
                    return PlateAppearanceOutcome.GroundOut;
                default:
                    return PlateAppearanceOutcome.Strikeout;
            }
        }

        private static decimal SafePct(int num, int denom)
        {
            if (denom <= 0) {
                return 0;
            }

            return (1.0m * num) / (1.0m * denom);
        }
    }
}