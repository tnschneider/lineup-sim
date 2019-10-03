using System;

namespace lineupSim.Models 
{
    public enum BaseState
    {
        Empty,
        First,
        Second,
        Third,
        FirstAndSecond,
        FirstAndThird,
        SecondAndThird,
        Full
    }

    public static class BaseStateExtensions
    {
        public static bool RunnerOnFirst(this BaseState baseState)
        {
            return baseState.In(BaseState.First, BaseState.FirstAndSecond, BaseState.FirstAndThird, BaseState.Full);
        }

        public static bool RunnerOnSecond(this BaseState baseState)
        {
            return baseState.In(BaseState.Second, BaseState.FirstAndSecond, BaseState.SecondAndThird, BaseState.Full);
        }

        public static bool RunnerOnThird(this BaseState baseState)
        {
            return baseState.In(BaseState.Third, BaseState.FirstAndThird, BaseState.SecondAndThird, BaseState.Full);
        }

        public static int RunnersOnBase(this BaseState baseState)
        {
            if (baseState == BaseState.Empty)
            {
                return 0;
            }
            else if (baseState.In(BaseState.First, BaseState.Second, BaseState.Third))
            {
                return 1;
            }
            else if (baseState.In(BaseState.FirstAndSecond, BaseState.SecondAndThird, BaseState.FirstAndThird))
            {
                return 2;
            }

            return 3;
        }
    }
}