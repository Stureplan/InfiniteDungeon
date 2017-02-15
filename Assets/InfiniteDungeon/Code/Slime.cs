using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Agent
{
    private bool inAttackRange = false;
    Cell targetCell;

    public override void Damage(int damage)
    {
        health -= damage;
        if (health < 1)
        {
            Kill();
        }
    }

    public override void Kill()
    {
        cell.occupant = 0;
        cell.enemy = null;
        Destroy(gameObject);
    }

    public override void NextTurn(int turn)
    {
        SLIME_MOVE_TYPE type = DecideMove(turn);
        ExecuteMove(type, turn);
        VisualTurn(type, turn);
    }

    public SLIME_MOVE_TYPE DecideMove(int turn)
    {
        Cell[] cells = Pathfinder.FindPath(barbarian.cell, cell);
        SLIME_MOVE_TYPE move;

        if (cells.Length > 2)
        {
            targetCell = cells[0];
            move = SLIME_MOVE_TYPE.MOVE;
        }
        else
        {
            targetCell = barbarian.cell;
            move = SLIME_MOVE_TYPE.ATTACK;
        }

        return move;
    }

    public void ExecuteMove(SLIME_MOVE_TYPE type, int turn)
    {
        switch(type)
        {
            case SLIME_MOVE_TYPE.MOVE:
                cell.occupant = 0;
                cell.enemy = null;
                cell = targetCell;
                cell.occupant = 1;
                cell.enemy = this;
                break;

            case SLIME_MOVE_TYPE.ATTACK:
                barbarian.Damage(5);
                break;

            case SLIME_MOVE_TYPE.INVALID:

                break;
        }
    }

    public void VisualTurn(SLIME_MOVE_TYPE type, int turn)
    {
        switch(type)
        {
            case SLIME_MOVE_TYPE.MOVE:
                //PlayAnimation("Slime_Move");
                StartCoroutine(Move(cell.position, 0.2f));
                StartCoroutine(Rotate(QHelp.QDIR(cell.position - transform.position), 0.1f));
                break;

            case SLIME_MOVE_TYPE.ATTACK:
                //PlayAnimation("Slime_Attack");
                break;

            case SLIME_MOVE_TYPE.INVALID:

                break;
        }
    }


    private void Start()
    {
        map = Map.FindMap();
        barbarian = Barbarian.FindBarbarian();
    }

    private IEnumerator Move(Vector3 end, float time)
    {
        float t = 0.0f;

        while (t < time)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, end, t / time);
            t += Time.deltaTime;

            yield return null;
        }
    }

    private IEnumerator Rotate(Quaternion end, float time)
    {
        float t = 0.0f;

        while (t < time)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, end, t / time);
            t += Time.deltaTime;

            yield return null;
        }
    }
}
