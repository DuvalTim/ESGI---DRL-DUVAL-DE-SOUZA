using System;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.DRL;
using Environment = Assets.DRL.Environment;
using Random = UnityEngine.Random;

public class TestScript : MonoBehaviour
{

    private readonly string specifier = "E01";
    private readonly CultureInfo culture = CultureInfo.CreateSpecificCulture("sv-SE");
    public GameObject Player;

    public uint MapDimension = 4;
    public float UpdateTime;
    private float LastUpdate = 0.0f;

    private Environment Environment = null;
    private Agent Agent;
    private State[,] States;
    private List<Vector2> PreviousObstacleList;
    private bool Play = false;
    public bool DisplayReward = true;

    [SerializeField]
    private GameObject CaseDisplayPrefab;

    [SerializeField]
    private GameObject CanvasObject;

    private List<CaseDisplay> CaseDisplay = new List<CaseDisplay>();


    [SerializeField]
    private Text DebugText;

    private float gamma = 1.0f;

    [SerializeField]
    private Slider GammaSliderDecrease;

    [SerializeField]
    private Slider StartValuePolicySlider;

    [SerializeField]
    private Slider UpdateTimeSlider;

    [SerializeField]
    private GameObject StartGameObject;

    [SerializeField]
    private Dropdown AlgoSelected;
    [SerializeField]
    private Dropdown ObsSelected;

    private Button StartButton;
    private Text StartText;

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
        StartButton = StartGameObject.GetComponent<Button>();
        StartText = StartGameObject.GetComponentInChildren<Text>();
        GammaSliderDecrease.onValueChanged.AddListener(OnSliderValueChanged);
        StartValuePolicySlider.onValueChanged.AddListener(OnSliderValueChanged);
        UpdateTimeSlider.onValueChanged.AddListener(OnSliderValueChanged);
        if (StartButton == null || StartText == null)
        {
            Debug.LogError("Null start button");
        }
        else
        {
            StartText.text = "Start";
            StartButton.onClick.AddListener(OnStartButtonClick);
        }
    }

    private void OnPreCull()
    {

    }
    private void OnStartButtonClick()
    {
        Play = !Play;
        if (Play)
        {
            InstantiateDisplay();
            StartGame();
        }
        else
        {
            Play = !Play;
            StartText.text = "------";
            StartCoroutine(AutoRestart());
        }
        gamma = 1.0f;

        
        switch (AlgoSelected.value)
        {
            case 0:
                Debug.Log("Starting InitPolicy");
                InitPolicy();
                PolicyEvaluation();
                break;
            case 1:
                Debug.Log("Starting ValueIterationProcess");
                ValueIterationProcess(); 
                break;
            default:
                // code block
                break;
        }


        // pol it
        //InitPolicy();
        //PolicyEvaluation();

        // pol it
        
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
                CaseDisplay[i * States.GetLength(0) + j].SetText(FloatToString(States[i, j].Value));
            }
        }
    }

    private string FloatToString(float value)
    {
        return Mathf.Abs(value) > 0.01f ? "" + Math.Round(value, 2) : value.ToString(specifier, culture);
    }

    private void OnSliderValueChanged(float value)
    {
        DebugText.text = FloatToString(value);
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
        StartText.text = "Restart";
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

        switch (ObsSelected.value)
        {
            case 2:
                Debug.Log("No Obstacle");
                break;
            case 0:
                Debug.Log("Random Obstacle");
                RandomObstacles();
                break;
            case 1:
                Debug.Log("Previous Obstacle");
                UsePreviousObstacles();
                break;
            default:
                // code block
                break;
        }

        /*
        if (ObstacleToggle.isOn)
        {
            RandomObstacles();
        }
        */
       


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
                StartText.text = "Start";
                UpdatePlayerPosition();
            }
        }
    }

    private void RandomObstacles()
    {
       List<Vector2> ObstacleList = new List<Vector2>();
        for (int i = 2; i < States.GetLength(0) - 1; i++)
        {
            for (int j = 2; j < States.GetLength(1) - 1; j++)
            {
                if (Random.Range(0, 10) < 1.5f)
                {
                    States[i, j].DefineAsObstacle(-1);
                    ObstacleList.Add(new Vector2(i, j));
                }
            }
        }
        PreviousObstacleList = new List<Vector2>(ObstacleList);
    }

    private void UsePreviousObstacles()
    {
        foreach(Vector2 c in PreviousObstacleList)
        {
            States[(int)c[0], (int)c[1]].DefineAsObstacle(-1);
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

    private void ValueIterationProcess()
    {
        InitValueIt();
        gamma = STARTGAMMA;
        float delta;
        uint count = 0;
        do
        {
            delta = .0f;

            for (int i = 0; i < States.GetLength(0); i++)
            {
                for (int j = 0; j < States.GetLength(1); j++)
                {
                    float temp = States[i, j].Value;
                    States[i, j].GetMaxRewardStateValIt(gamma, out float value);
                    States[i, j].Value = value;
                    delta = Mathf.Max(delta, Mathf.Abs(temp - States[i, j].Value));
                }
            }
            gamma = Mathf.Max(0.01f, gamma - GammaSliderDecrease.value);
            count++;
        } while (delta > THETA && count < MAX_ITERATION);
        Debug.Log("COunt " + count);
        Debug.Log("delta " + delta);
    }

    public float MAX_ITERATION = 1000;
    public float THETA = 0.001f;
    public float STARTGAMMA = .9f;
    private void InitValueIt()
    {
        for (int i = 0; i < States.GetLength(0); i++)
        {
            for (int j = 0; j < States.GetLength(1); j++)
            {
                States[i, j].Value = StartValuePolicySlider.value;
                //States[i, j].RandomPolicy();
            }
        }
    }
}
