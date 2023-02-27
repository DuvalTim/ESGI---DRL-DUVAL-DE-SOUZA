using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TestScript : MonoBehaviour
{
    public GameObject Player;

    public uint MapDimension = 4;
    public float UpdateTime;
    private float LastUpdate = 0.0f;

    private Environment Environment = null;
    private Agent Agent;
    private State[,] States;
    private bool Play = false;
    public bool DisplayReward = true;

    [SerializeField]
    private GameObject CaseDisplayPrefab;
    [SerializeField]
    private GameObject CanvasObject;
    private List<CaseDisplay> CaseDisplay = new List<CaseDisplay>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Play = !Play;
            if (Play)
            {
                InstantiateDisplay();
                StartGame();
            }
        }
        else
        {
            if (UpdateTime != 0 && UpdateTime + LastUpdate < Time.time)
            {
                LastUpdate = Time.time;
                StepGame();
            }
        }
        if (Input.GetKeyDown(KeyCode.I)) InitPolicy();
        if (Input.GetKeyDown(KeyCode.E))
        {
            gamma = 1.0f;
            PolicyEvaluation();
        }
    }

    private void InstantiateDisplay()
    {
        var value = MapDimension * MapDimension;
        if (CaseDisplay.Count < value)
        {
            for (int i = CaseDisplay.Count; i < value; i++)
            {
                GameObject obj = Instantiate<GameObject>(CaseDisplayPrefab, CanvasObject.transform);

                CaseDisplay.Add(new CaseDisplay(obj, obj.GetComponentInChildren<RawImage>(), obj.GetComponentInChildren<Text>()));
                obj.SetActive(false);
            }
        }
        for (int i = 0; i < (int)(MapDimension * MapDimension); i++)
        {
            CaseDisplay[i].SetActive(true);
        }
        for (int i = (int)(MapDimension * MapDimension); i < CaseDisplay.Count; i++)
        {
            CaseDisplay[i].SetActive(false);
        }
    }

    private void OnGUI()
    {
        if (Environment != null)
        {
            UpdatePlayerPosition();
            DisplayValues();
        }
    }

    private void DisplayValues()
    {
        for (int i = 0; i < States.GetLength(0); i++)
        {
            for (int j = 0; j < States.GetLength(1); j++)
            {
                CaseDisplay[i * States.GetLength(0) + j].SetName(States[i, j].Name);
                CaseDisplay[i * States.GetLength(0) + j].SetPosition(new Vector3(i, j, 0));
                CaseDisplay[i * States.GetLength(0) + j].SetColor(States[i, j].Reward == 1 ? Color.green : (States[i, j].Reward == 0 ? Color.yellow : Color.red));
                CaseDisplay[i * States.GetLength(0) + j].SetText("" + Math.Round(States[i, j].Value, 2));
            }
        }
    }

    private void UpdatePlayerPosition()
    {
        for (int i = 0; i < States.GetLength(0); i++)
        {
            for (int j = 0; j < States.GetLength(1); j++)
            {
                if (Environment.m_Agent.State == States[i, j])
                {
                    Player.transform.position = new Vector3(i, 0, j);
                }
            }
        }
    }

    private void StepGame()
    {
        if (Play)
        {
            var val = Environment.Step();
            if (val)
            {
                Play = false;
                UpdatePlayerPosition();
            }
        }
    }

    private void StartGame()
    {
        Agent = new Agent();
        States = new State[MapDimension, MapDimension];
        for (int i = 0; i < States.GetLength(0); i++)
        {
            for (int j = 0; j < States.GetLength(1); j++)
            {
                States[i, j] = new State(0, "" + i + "x" + j);
            }
        }
        for (int i = 0; i < States.GetLength(0); i++)
        {
            for (int j = 0; j < States.GetLength(1); j++)
            {
                if (i == States.GetLength(0) - 1 && j == States.GetLength(1) -1)
                {
                    break;
                }
                //States[i, j] = new State(0, "" + i + "x" + j);
                if (i != 0)
                {
                    States[i, j].AddReachableStates(States[i - 1, j]);
                }
                if (i != States.GetLength(0) - 1)
                {
                    States[i, j].AddReachableStates(States[i + 1, j]);
                }
                if (j != 0)
                {
                    States[i, j].AddReachableStates(States[i, j - 1]);
                }
                if (j != States.GetLength(1) - 1)
                {
                    States[i, j].AddReachableStates(States[i, j + 1]);
                }
            }
        }
        States[States.GetLength(0) - 1, States.GetLength(1) - 1].Reward = 1.0f;
        RandomObstacles();
        Environment = new Environment(States, States[0, 0], new State[] { States[States.GetLength(0) - 1, States.GetLength(1) - 1] });
        Environment.Init(Agent);
        Play = true;
        //Environment.OnEndStateReached = new Action(() => { 
        //    Play = false; 

        //});
    }

    private void RandomObstacles()
    {
        for (int i = 0; i < States.GetLength(0) - 1; i++)
        {
            for (int j = 0; j < States.GetLength(1) - 1; j++)
            {
                if (Random.Range(0, 10) < 1.5f)
                {
                    States[i, j].Reward = -1;
                }
            }
        }
    }

    private void InitPolicy()
    {
        float baseValue = Random.Range(0, .5f);
        for (int i = 0; i < States.GetLength(0); i++)
        {
            for (int j = 0; j < States.GetLength(1); j++)
            {
                States[i, j].Value = baseValue;
                States[i, j].RandomPolicy();
            }
        }
    }

    private float gamma = 1.0f;

    private void PolicyEvaluation()
    {
        uint count = 0;
        float delta;
        do
        {
            delta = 0;
            for (int i = 0; i < States.GetLength(0); i++)
            {
                for (int j = 0; j < States.GetLength(1); j++)
                {
                    float temp = States[i, j].Value;
                    States[i, j].Value = States[i, j].GetFuturValue(gamma);
                    delta = Mathf.Max(delta, Mathf.Abs(temp - States[i, j].Value));
                }
            }
            gamma = Mathf.Max(0.01f, gamma - 0.005f);
            count++;
        } while (delta < 0.01f && count < 50000);
        Debug.Log("COunt " + count);
        Debug.Log("delta " + delta);
        PolicyOptimisation();
    }

    private void PolicyOptimisation()
    {

        for (int i = 0; i < States.GetLength(0); i++)
        {
            for (int j = 0; j < States.GetLength(1); j++)
            {
                var temp = States[i, j].BestActionState;
                States[i, j].BestActionState = States[i, j].BestValueArgMax();
                if (temp != States[i, j].BestActionState)
                {
                    Debug.Log("Not optimal");
                    PolicyEvaluation();
                }
            }
        }
    }
}

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

    public State GetRandomAction()
    {
        if (ReachableStates?.Count != 0)
        {
            return ReachableStates[UnityEngine.Random.Range(0, ReachableStates.Count)];
        }
        return this;
    }

    internal State BestValueArgMax()
    {
        if (ReachableStates?.Count != 0)
        {
            State res = ReachableStates[0];
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

    internal void RandomPolicy()
    {
        BestActionState = ReachableStates?.Count != 0 ? ReachableStates[Random.Range(0, ReachableStates.Count)] : this;
    }

    internal float GetFuturValue(float gamma)
    {
        var e = Reward;
        return e + gamma * (BestActionState == this ? Value : BestActionState.Value);
    }

    public static bool operator ==(State state1, State state2)
    {
        return state1.Name == state2.Name;
    }


    public static bool operator !=(State state1, State state2)
    {
        return !(state1 == state2);
    }
}

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

public class Environment
{
    public State[,] AllStates;
    public State BeginState;
    public State[] EndStates;
    public Agent m_Agent;
    public Action OnEndStateReached;

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

    public bool Step()
    {
        State newState = m_Agent.State.GetRandomAction();
        m_Agent.DoAction(newState);
        for (int i = 0; i < EndStates.Length; i++)
        {
            if (newState == EndStates[i])
            {
                OnEndStateReached?.Invoke();
                return true;
            }
        }
        return false;
    }
}
