using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                    Vector2 NextAction = ActionManager(x, y);
                    MapValue[x, y] = MapValue[(int)NextAction.x, (int)NextAction.y];
                    delta = (int)Mathf.Max(delta, Mathf.Abs(tempVal - MapValue[x, y]));
                }
            }       
        }
        
    }

    public void Policy_Improvement()
    {
        bool inStable = true;

        while (inStable)
        {
            for (int x = 0; x < 18; x++)
            {
                for (int y = 0; y < 18; y++)
                {
                    float tempPi = MapValue[x, y];
                    //recup tout les voisins
                    for (int a = 1; a < 5; a++)
                    {

                    }

                    Vector2 NextAction = ActionManager(x, y);
                    MapValue[x, y] = MapValue[(int)NextAction.x, (int)NextAction.y];



                   //delta = (int)Mathf.Max(delta, Mathf.Abs(tempVal - MapValue[x, y]));
                }
            }
        }

    }

    public Vector2 ActionManager(int x, int y)
    {
        /*  1 = Up
        *   2 = Right
        *   3 = Down
        *   4 = Left
        */

        switch (MapAction[x, y])
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
