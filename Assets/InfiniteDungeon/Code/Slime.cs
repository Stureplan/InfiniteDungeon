using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Agent
{
    delegate void EnemyAction();
    EnemyAction action;

    public override void NextTurn(Map map, int turn)
    {
        DecideMove(map, turn);
        ExecuteMove(map, turn);
    }

    public override void DecideMove(Map map, int turn)
    {
        action = Test;
    }

    public override void ExecuteMove(Map map, int turn)
    {
        action();
    }

    public override void VisualTurn(Map map, int turn) { }


    void Test()
    {

    }

    void OtherTest()
    {

    }    
}
