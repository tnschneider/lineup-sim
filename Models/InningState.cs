using System.Linq;

namespace lineupSim.Models 
{
    public class InningState
    {
        public int Outs { get; set; }
        public BaseState BaseState { get; set; }
        public int Score { get; set; }
        public bool Completed => Outs > 2;

        public static InningState Initial => new InningState
        {
            Outs = 0,
            BaseState = BaseState.Empty,
            Score = 0
        };

        public InningState Transition(PlateAppearanceOutcome outcome)
        {
            var (rbis, newBaseState, outs) = GetNewValues(outcome, BaseState, Outs);
            
            this.Score += rbis;
            this.BaseState = newBaseState;
            this.Outs = outs;

            return this;
        }

        private (int rbis, BaseState newBaseState, int outs) GetNewValues(PlateAppearanceOutcome outcome, BaseState baseState, int outs)
        {
            var rbis = 0;
            var newBaseState = baseState;

            switch (outcome)
            {
                case PlateAppearanceOutcome.Strikeout:
                    outs++;
                    break;
                case PlateAppearanceOutcome.Walk:
                    if (baseState == BaseState.Full)
                        rbis++;
                    newBaseState = HandleWalk(baseState);
                    break;
                case PlateAppearanceOutcome.Single:
                    if (baseState.RunnerOnSecond())
                        rbis++;
                    if (baseState.RunnerOnThird())
                        rbis++;
                    newBaseState = HandleSingle(baseState);
                    break;
                case PlateAppearanceOutcome.Double:
                    rbis = baseState.RunnersOnBase();
                    newBaseState = BaseState.Second;
                    break;
                case PlateAppearanceOutcome.Triple:
                    rbis = baseState.RunnersOnBase();
                    newBaseState = BaseState.Third;
                    break;
                case PlateAppearanceOutcome.HomeRun:
                    rbis = baseState.RunnersOnBase() + 1;
                    newBaseState = BaseState.Empty;
                    break;
                case PlateAppearanceOutcome.FlyOut:
                    var sf = baseState.RunnerOnThird() && outs < 2;
                    
                    if (sf) 
                        rbis++;
                    
                    newBaseState = HandleFlyOut(baseState);
                    outs++;
                    break;
                case PlateAppearanceOutcome.GroundOut:
                    var first = baseState.RunnerOnFirst();
                    var second = baseState.RunnerOnSecond();
                    var third = baseState.RunnerOnThird();

                    var dp = (first || second) && !third;
                    
                    if (dp) 
                    {
                        outs += 2;
                    }
                    else
                    {
                        outs++;
                    }

                    newBaseState = HandleGroundOut(baseState);
                    
                    break;
            }

            return (rbis, newBaseState, outs);
        }

        private BaseState HandleWalk(BaseState state)
        {
            switch (state)
            {
                case BaseState.Empty:
                    return BaseState.First;
                case BaseState.First:
                    return BaseState.FirstAndSecond;
                case BaseState.Second:
                    return BaseState.FirstAndSecond;
                case BaseState.Third:
                    return BaseState.FirstAndThird;
                case BaseState.FirstAndSecond:
                    return BaseState.Full;
                case BaseState.FirstAndThird:
                    return BaseState.Full;
                case BaseState.SecondAndThird:
                    return BaseState.Full;
                case BaseState.Full:
                    return BaseState.Full;
                default: 
                    return state;
            }
        }

        private BaseState HandleSingle(BaseState state)
        {
            switch (state)
            {
                case BaseState.Empty:
                    return BaseState.First;
                case BaseState.First:
                    return BaseState.FirstAndThird;
                case BaseState.Second:
                    return BaseState.First;
                case BaseState.Third:
                    return BaseState.First;
                case BaseState.FirstAndSecond:
                    return BaseState.FirstAndThird;
                case BaseState.FirstAndThird:
                    return BaseState.FirstAndThird;
                case BaseState.SecondAndThird:
                    return BaseState.First;
                case BaseState.Full:
                    return BaseState.FirstAndThird;
                default: 
                    return state;
            }
        }

        private BaseState HandleFlyOut(BaseState state)
        {
            switch (state)
            {
                case BaseState.Second:
                    return BaseState.Third;
                case BaseState.Third:
                    return BaseState.Empty;
                case BaseState.FirstAndSecond:
                    return BaseState.FirstAndThird;
                case BaseState.FirstAndThird:
                    return BaseState.First;
                case BaseState.SecondAndThird:
                    return BaseState.Third;
                case BaseState.Full:
                    return BaseState.FirstAndThird;
                default: 
                    return state;
            }
        }

        private BaseState HandleGroundOut(BaseState state)
        {
            switch (state)
            {
                case BaseState.First:
                    return BaseState.Empty;
                case BaseState.Second:
                    return BaseState.Empty;
                case BaseState.Third:
                    return BaseState.First;
                case BaseState.FirstAndSecond:
                    return BaseState.First;
                case BaseState.FirstAndThird:
                    return BaseState.FirstAndSecond;
                case BaseState.SecondAndThird:
                    return BaseState.FirstAndThird;
                case BaseState.Full:
                    return BaseState.Full;
                default: 
                    return state;
            }
        }
    }
}