using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Map map;

    public void NextTurn(int turn) { }
    public void DecideMove(int turn) { }
    public void ExecuteMove(int turn) { }
    public float VisualTurn(int turn)
    {
        // Handle animations, effects etc.

        return 1.0f;
    }
}
