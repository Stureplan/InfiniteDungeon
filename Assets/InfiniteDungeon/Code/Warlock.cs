using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warlock : Agent
{
    private AnimatedObject anim;


    private Cell targetCell;

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
        Game.RemoveEnemy(this);

        anim.PlayAnimation("Warlock_Death");
        FX.Emit(transform.localPosition + Game.HALF_Y, Quaternion.identity, FX.VFX.Bats, 15);

        StartCoroutine(DelayedDestruction(anim.GetClipLength("Warlock_Death")));
    }

    public override void PushBack(Cell c)
    {
        int x = cell.x - c.x;
        int y = cell.y - c.y;

        Cell target = map.CellAtPoint(cell.x + x, cell.y + y);

        if (target.type == 0 && target.occupant == 0)
        {
            cell.occupant = 0;
            cell.enemy = null;
            cell = target;
            cell.occupant = 1;
            cell.enemy = this;

            anim.PlayAnimation("Warlock_Move");
            StartCoroutine(Move(target.position, 0.2f));
        }
    }

    public override void SetupEnemy(Cell c)
    {
        c.enemy = this;
        c.occupant = 1;
        cell = c;

        Initialize();
    }

    public override void NextTurn(int turn)
    {
        WARLOCK_MOVE_TYPE type = DecideMove(turn);
        ExecuteMove(type, turn);
        VisualTurn(type, turn);
    }

    public WARLOCK_MOVE_TYPE DecideMove(int turn)
    {
        Cell[] cells = Pathfinder.FindPath(barbarian.cell, cell);
        WARLOCK_MOVE_TYPE move;

        
        if (cells.Length > 1) // If we're further than 1 tile away
        {
            targetCell = cells[0];
            move = WARLOCK_MOVE_TYPE.MOVE;
        }
        else                 // Else we're in range of melee attack or blocked and isolated
        {
            if (cells[0] == cell)
            {
                //We're blocked
                move = WARLOCK_MOVE_TYPE.NOTHING;
            }
            else
            {
                targetCell = barbarian.cell;
                move = WARLOCK_MOVE_TYPE.ATTACK;
            }
        }

        if (cells.Length > 2 && cells.Length < 6) // If Barb is in spell range
        {
            // If the path lines up either horizontally or vertically
            if (Map.CellsAreAlignedH(cells, cell.x) || Map.CellsAreAlignedV(cells, cell.y))
            {
                // We're gonna cast a Shadow Bolt
                targetCell = barbarian.cell;
                move = WARLOCK_MOVE_TYPE.SPELL1;
            }
        }

        return move;
    }

    private void ExecuteMove(WARLOCK_MOVE_TYPE type, int turn)
    {
        switch (type)
        {
            case WARLOCK_MOVE_TYPE.MOVE:
                cell.occupant = 0;
                cell.enemy = null;
                cell = targetCell;
                cell.occupant = 1;
                cell.enemy = this;
                break;

            case WARLOCK_MOVE_TYPE.ATTACK:
                barbarian.Damage(5);
                break;

            case WARLOCK_MOVE_TYPE.SPELL1:
                GameObject go = FX.ShadowBolt(transform.position + Game.HALF_Y, Quaternion.LookRotation((targetCell.position-cell.position).normalized));
                go.AddComponent<ShadowBolt>().Init(targetCell);
                break;  

            case WARLOCK_MOVE_TYPE.NOTHING:

                break;
        }
    }

    private void VisualTurn(WARLOCK_MOVE_TYPE type, int turn)
    {
        switch (type)
        {
            case WARLOCK_MOVE_TYPE.MOVE:
                anim.PlayAnimation("Warlock_Move");
                StartCoroutine(Move(cell.position, 0.2f));
                StartCoroutine(Rotate(Helper.QDIR(cell.position - transform.position), 0.1f));
                break;

            case WARLOCK_MOVE_TYPE.ATTACK:
                Debug.Log("Attack");
                anim.PlayAnimation("Warlock_Attack");
                StartCoroutine(Rotate(Helper.QDIR(targetCell.position - transform.position), 0.1f));
                break;

            case WARLOCK_MOVE_TYPE.SPELL1:
                anim.PlayAnimation("Warlock_Spell1");
                StartCoroutine(Rotate(Helper.QDIR(targetCell.position - transform.position), 0.1f));
                break;

            case WARLOCK_MOVE_TYPE.NOTHING:

                break;
        }
    }

    private void Initialize()
    {
        map = Map.FindMap();
        barbarian = Barbarian.FindBarbarian();
        anim = GetComponent<AnimatedObject>();
        anim.Initialize();
        anim.PlayAnimation("Warlock_Idle");

        value = 5;
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

    private IEnumerator DelayedDestruction(float time)
    {
        float t = 0.0f;

        while (t < time)
        {
            t += Time.deltaTime;

            yield return null;
        }

        HandleMoney();
        Destroy(gameObject);
    }
}
