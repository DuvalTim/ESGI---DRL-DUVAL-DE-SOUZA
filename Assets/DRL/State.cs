using System;
using System.Collections.Generic;
namespace Assets.DRL
{
    public class State
    {
        public string Name;
        public List<State> ReachableStates;
        public float Reward;
        public float Value { get; set; }
        public State BestActionState;

        public State(float reward, string name, float value = 0)
        {
            Reward = reward;
            Name = name;
            ReachableStates = new List<State>();
            Value = value;
        }

        public void AddReachableStates(State reachableStates)
        {
            ReachableStates.Add(reachableStates);
        }


        internal void DefineAsObstacle(int v)
        {
            Reward = v;
            ReachableStates = new List<State>();
        }

        public State GetRandomAction()
        {
            if (ReachableStates?.Count != 0)
            {
                return ReachableStates[UnityEngine.Random.Range(0, ReachableStates.Count)];
            }
            return this;
        }

        internal State MoveToBestValueArgMax()
        {
            if (ReachableStates?.Count != 0)
            {
                State res = ReachableStates[UnityEngine.Random.Range(0, ReachableStates.Count)];
                for (int i = 1; i < ReachableStates.Count; i++)
                {
                    if (res.Value < ReachableStates[i].Value)
                    {
                        res = ReachableStates[i];
                    }
                }
                return res;
            }
            return this;
        }

        internal State BestValueArgMax()
        {
            if (ReachableStates?.Count != 0)
            {
                State res = ReachableStates[0];
                for (int i = 0; i < ReachableStates.Count; i++)
                {
                    if (res.Value < ReachableStates[i].Value)
                    {
                        res = ReachableStates[i];
                    }
                }
                return res;
            }
            return this;
        }

        internal void RandomPolicy()
        {
            BestActionState = ReachableStates?.Count != 0 ? ReachableStates[UnityEngine.Random.Range(0, ReachableStates.Count)] : this;
        }

        internal float GetFuturValuePolIt(float gamma)
        {
            var e = Reward;
            return e + gamma * (BestActionState == this ? Value : BestActionState.Value);
        }

        internal float GetMaxRewardStateValItRec(float gamma)
        {

            if (ReachableStates?.Count != 0)
            {
                State bState = ReachableStates[0];
                float res = bState.GetMaxRewardStateValItRec(gamma);
                for (int i = 1; i < ReachableStates.Count; i++)
                {
                    float newRes = ReachableStates[i].GetMaxRewardStateValItRec(gamma);
                    if (newRes > res)
                    {
                        res = newRes;
                    }
                }
                return res;
            }
            else
            {
                return this.Reward + gamma * this.Value;
            }
        }

        
        internal State GetMaxRewardStateValIt(float gamma, out float value)
        {
            if (ReachableStates?.Count != 0)
            {
                State bState = ReachableStates[0];
                value = bState.Reward + gamma * bState.Value;
                for (int i = 1; i < ReachableStates.Count; i++)
                {
                    if (value < ReachableStates[i].Reward + gamma * ReachableStates[i].Value)
                    {
                        bState = ReachableStates[i];
                        value = bState.Reward + gamma * bState.Value;
                    }
                }
                return bState;
            }
            value = this.Reward + gamma * this.Value;
            return this;
        }

        
        //internal void GetMaxRewardStateValIt(float gamma, out float value)
        //{
        //    value = Reward;
        //    if (ReachableStates?.Count != 0)
        //    {
        //        for (int i = 0; i < ReachableStates.Count; i++)
        //        {
        //            value += /* 1 / ReachableStates.Count * */ gamma * ReachableStates[i].Value;
        //        }
        //    }
        //} 
         

        public static bool operator ==(State state1, State state2)
        {
            return state1.Name == state2.Name;
        }


        public static bool operator !=(State state1, State state2)
        {
            return !(state1 == state2);
        }
    }

}
