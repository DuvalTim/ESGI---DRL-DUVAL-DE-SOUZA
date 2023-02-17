using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PolicyIteration : MonoBehaviour
{   //      x           y      z
    //Map : Recompense, Value, Action

    public class Case
    {

        public float Reward = 0;
        public float Value = 0;
        public float Action = 0;
    }

    public Case[,] Map = new Case[18, 18];


    void Start()
    {
        //Sortie
        Map[17, 17].Reward = 1;
        Map[17, 17].Value = 1;

    }
    public void Policy_Evaluation()
    {
        int delta = 0;

        //Randomize Action
        for (int x = 0; x < 17; x++)
        {
            for (int y = 0; y < 17; y++)
            {
                Map[x, y].Action = (Random.Range(1, 4));
            }
        }


        while (delta < 0.01f)
        {
            delta = 0;
            for (int x = 0; x < 18; x++)
            {
                for (int y = 0; y < 18; y++)
                {
                    float tempVal = Map[x, y].Value;
                    Vector2 NextAction = ActionManager(x, y, Map);
                    Map[x, y].Value = Map[(int)NextAction.x, (int)NextAction.y].Value;
                    delta = (int)Mathf.Max(delta, Mathf.Abs(tempVal - Map[x, y].Value));
                }
            }
        }

    }

    public void Policy_Improvement()
    {
        for (int x = 0; x < 18; x++)
        {
            for (int y = 0; y < 18; y++)
            {
                for (int a = 1; a < 5; a++)
                {
                    
                }
            }
        }
    }

    public Vector2 ActionManager(int x, int y, Case[,] mapAction)
    {
        /*  1 = Up
        *   2 = Right
        *   3 = Down
        *   4 = Left
        */

        switch (mapAction[x, y].Action)
        {
            case 1:
                if (y + 1 <= 17) y++;
                break;
            case 2:
                if (x + 1 <= 17) x++;
                break;
            case 3:
                if (y - 1 >= 0) y--;
                break;
            case 4:
                if (y - 1 >= 0) x--;
                break;
            default:
                break;
        }
        return new Vector2(x,y);
    }

    

}

/*
public class PolicyIteration : MonoBehaviour
{
    private float[,] Map = new float[18, 18];

    private float[,] MapRecompense = new float[18, 18];
    private float[,] MapValue = new float[18, 18];
    private float[,] MapAction = new float[18, 18];

    void Start()
    {
        //Sortie
        MapValue[17, 17] = 1;

        //Randomize MapAction
        for (int x = 0; x < 17; x++)
        {
            for (int y = 0; y < 17; y++)
            {
                MapAction[x, y] = (Random.Range(1, 4));

            }
        }
    }

    public void Policy_Evaluation()
    {
        int delta = 0;

        while (delta<0.01f)
        {
            delta = 0;
            for(int x = 0; x < 18; x++)
            {
                for(int y = 0; y < 18; y++)
                {
                    float tempVal = MapValue[x,y];
                    Vector2 NextAction = ActionManager(x, y, MapAction);
                    MapValue[x, y] = MapValue[(int)NextAction.x, (int)NextAction.y];
                    delta = (int)Mathf.Max(delta, Mathf.Abs(tempVal - MapValue[x, y]));
                }
            }       
        }
        
    }

    public void Policy_Improvement()
    {
        int py = 0;
        float[,] tempMV = MapValue;

        for (int x = 0; x < 18; x++)
        {
            for (int y = 0; y < 18; y++)
            {
                int oldvalue = 0;
                for (int a = 1; a < 5; a++)
                {
                    tempMV[x, y] = a;
                    Vector2 testAction = ActionManager(x, y, MapAction);
                    if (oldvalue < tempMV[(int)testAction.x, (int)testAction.y])
                }
            }
        }
    }

    /*
    public void Policy_Improvement()
    {
        bool Stable = true;
        float[,] TempTab = MapValue;
        float[,] TempTabAction = MapAction;

        for (int x = 0; x < 18; x++)
        {
            for (int y = 0; y < 18; y++)
            {
                float tempPi = MapValue[x, y];
                //recup tout les voisins
                for (int a = 1; a < 5; a++)
                {
                    if (TempTab[x, y] < MapValue[(int)ActionManager(x, y, TempTabAction).x, (int)ActionManager(x, y, TempTabAction).y])
                        TempTab[x, y] = MapValue[(int)ActionManager(x, y, TempTabAction).x, (int)ActionManager(x, y, TempTabAction).y];
                }
                if (tempPi != TempTab[x, y]) Stable = false;                
            }
        }
        if (Stable) 
        {
            MapValue = TempTab;
        }
        else Policy_Evaluation();
    }
    

public void printresult()
    {
        for (int x = 0; x < 18; x++)
        {
            for (int y = 0; y < 18; y++)
            {
                Debug.Log(MapValue[x, y]); Debug.Log(" , ");
            }
            Debug.Log("\n");
        }
    }

    public Vector2 ActionManager(int x, int y, float[,] actiontable)
    {
        /*  1 = Up
        *   2 = Right
        *   3 = Down
        *   4 = Left
        */
/*
        switch (actiontable[x, y])
        {
            case 1:
                if (y + 1 <= 17) y++;
                break;
            case 2:
                if (x + 1 <= 17) x++;
                break;
            case 3:
                if (y - 1 >= 0) y--;
                break;
            case 4:
                if (y - 1 >= 0) x--;
                break;
            default:
                break;
        }
        return new Vector2(x,y);
    }

}
*/