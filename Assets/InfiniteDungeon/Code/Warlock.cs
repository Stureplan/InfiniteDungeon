using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warlock : Agent
{
    private Animation animations;


    private Cell targetCell;

    public override void Damage(int damage)
    {
        health -= damage;
        if (health < 1)
        {
            Kill();
            dead = true;
        }
    }

    public override void Kill()
    {
        cell.occupant = 0;
        cell.enemy = null;
        Game.RemoveEnemy(this);
        Destroy(gameObject);
    }

    public override void SetupEnemy(Cell c)
    {
        c.enemy = this;
        c.occupant = 1;
        cell = c;
        dead = false;
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
        else                 // Else we're in range of melee attack
        {
            targetCell = barbarian.cell;
            move = WARLOCK_MOVE_TYPE.ATTACK;
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
                GameObject go = FX.Projectile(transform.position + Vector3.one / 2, Quaternion.LookRotation((targetCell.position - cell.position).normalized));
                go.AddComponent<ShadowBolt>().Init(targetCell, Map.Distance(cell, targetCell));
                break;

            case WARLOCK_MOVE_TYPE.INVALID:

                break;
        }
    }

    private void VisualTurn(WARLOCK_MOVE_TYPE type, int turn)
    {
        switch (type)
        {
            case WARLOCK_MOVE_TYPE.MOVE:
                PlayAnimation("Warlock_Move");
                StartCoroutine(Move(cell.position, 0.2f));
                StartCoroutine(Rotate(Helper.QDIR(cell.position - transform.position), 0.1f));
                break;

            case WARLOCK_MOVE_TYPE.ATTACK:
                Debug.Log("Attack");
                PlayAnimation("Warlock_Move");
                StartCoroutine(Rotate(Helper.QDIR(cell.position - transform.position), 0.1f));
                break;

            case WARLOCK_MOVE_TYPE.SPELL1:
                Debug.Log("Spell");
                PlayAnimation("Warlock_Move");
                StartCoroutine(Rotate(Helper.QDIR(targetCell.position - transform.position), 0.1f));
                //PlayAnimation("Warlock_Spell1");
                break;

            case WARLOCK_MOVE_TYPE.INVALID:

                break;
        }
    }

    private void PlayAnimation(string anim)
    {
        animations.Stop();
        animations.Play(anim);
    }

    private void Start()
    {
        map = Map.FindMap();
        barbarian = Barbarian.FindBarbarian();
        animations = GetComponent<Animation>();
        PlayAnimation("Warlock_Idle");
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
