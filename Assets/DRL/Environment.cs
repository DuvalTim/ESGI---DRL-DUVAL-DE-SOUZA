using System;

namespace Assets.DRL
{

    public class Environment
    {
        public State[,] AllStates;
        public State BeginState;
        public State[] EndStates;
        public Agent m_Agent;
        public event EventHandler OnEndStateReached;

        public Environment(State[,] allstates, State beginState, State[] endStates)
        {
            AllStates = allstates;
            BeginState = beginState;
            EndStates = endStates;
        }

        public void Init(Agent agent)
        {
            m_Agent = agent;
            m_Agent.Init(BeginState);
        }

        /// <summary>
        /// return true when finished
        /// </summary>
        /// <returns></returns>
        public bool BestValueStep()
        {
            //State newState = m_Agent.State.GetRandomAction();
            State newState = m_Agent.State.MoveToBestValueArgMax();
            m_Agent.DoAction(newState);
            for (int i = 0; i < EndStates.Length; i++)
            {
                if (newState == EndStates[i])
                {
                    OnEndStateReached?.Invoke(this, null);
                    return true;
                }
            }
            return false;
        }
    }

}
