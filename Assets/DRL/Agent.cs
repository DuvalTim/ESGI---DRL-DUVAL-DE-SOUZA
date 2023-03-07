using System.Collections.Generic;

namespace Assets.DRL
{

    public class Agent
    {
        public State State;
        public List<State> AllStatesDone;

        internal void Init(State beginState)
        {
            State = beginState;
            AllStatesDone = new List<State>();
            AllStatesDone.Add(beginState);
        }

        internal void DoAction(State newState)
        {
            State = newState;
            AllStatesDone.Add(newState);
        }
    }

}
