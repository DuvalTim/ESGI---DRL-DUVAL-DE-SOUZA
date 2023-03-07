using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.DRL;
using Environment = Assets.DRL.Environment;
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


    [SerializeField]
    private Toggle ObstacleToggle;

    [SerializeField]
    private Text DebugText;

    private float gamma = 1.0f;

    [SerializeField]
    private Slider GammaSliderDecrease;

    [SerializeField]
    private Slider StartValuePolicySlider;

    [SerializeField]
    private Slider UpdateTimeSlider;

    private void Update()
    {
        if (false)
        //if (Input.GetKeyDown(KeyCode.P))
        {
        }
        else
        {
            if (UpdateTimeSlider.value != 0 && UpdateTimeSlider.value + LastUpdate < Time.time)
            {
                LastUpdate = Time.time;
                StepGame();
            }
        }
    }

    private void Start()
    {
        GammaSliderDecrease.onValueChanged.AddListener(OnSliderValueChanged);
        StartValuePolicySlider.onValueChanged.AddListener(OnSliderValueChanged);
        UpdateTimeSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    public void StartPolItButtonClick()
    {
        Play = !Play;
        if (Play)
        {
            InstantiateDisplay();
            StartGame();
        }
        else
        {
            StartCoroutine(AutoRestart());
        }
        InitPolicy();
        gamma = 1.0f;
        PolicyEvaluation();
    }

    private System.Collections.IEnumerator AutoRestart()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        if (!Play)
        {
            Play = !Play;
            InstantiateDisplay();
            StartGame();
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

    private void OnSliderValueChanged(float value)
    {
        DebugText.text = $"{Math.Round(value, 3)}";
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
                if (i == States.GetLength(0) - 1 && j == States.GetLength(1) - 1)
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
        if (ObstacleToggle.isOn)
        {
            RandomObstacles();
        }
        Environment = new Environment(States, States[0, 0], new State[] { States[States.GetLength(0) - 1, States.GetLength(1) - 1] });
        Environment.Init(Agent);
        Play = true;
        //Environment.OnEndStateReached = new Action(() => { 
        //    Play = false; 

        //});
    }

    private void StepGame()
    {
        if (Play)
        {
            var val = Environment.PolItStep();
            if (val)
            {
                Play = false;
                UpdatePlayerPosition();
            }
        }
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
        //float baseValue = Random.Range(0, .5f);
        for (int i = 0; i < States.GetLength(0); i++)
        {
            for (int j = 0; j < States.GetLength(1); j++)
            {
                States[i, j].Value = StartValuePolicySlider.value;
                States[i, j].RandomPolicy();
            }
        }
    }

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
                    States[i, j].Value = States[i, j].GetFuturValuePolIt(gamma);
                    delta = Mathf.Max(delta, Mathf.Abs(temp - States[i, j].Value));
                }
            }
            gamma = Mathf.Max(0.01f, gamma - GammaSliderDecrease.value);
            count++;
        } while (delta < 0.01f && count < 10000);
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
