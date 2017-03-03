﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Agent
{
    private AnimatedObject anim;

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
        Game.RemoveEnemy(this);
        //Destroy(gameObject);

        anim.PlayAnimation("Slime_Death");
        StartCoroutine(DelayedDestruction(anim.GetClipLength("Slime_Death")));
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
        MELEE_MOVE_TYPE type = DecideMove(turn);
        ExecuteMove(type, turn);
        VisualTurn(type, turn);
    }



    public MELEE_MOVE_TYPE DecideMove(int turn)
    {
        Cell[] cells = Pathfinder.FindPath(barbarian.cell, cell);
        MELEE_MOVE_TYPE move;

        if (cells.Length > 1)
        {
            targetCell = cells[0];
            move = MELEE_MOVE_TYPE.MOVE;
        }
        else
        {
            targetCell = barbarian.cell;
            move = MELEE_MOVE_TYPE.ATTACK;
        }

        return move;
    }

    public void ExecuteMove(MELEE_MOVE_TYPE type, int turn)
    {
        switch(type)
        {
            case MELEE_MOVE_TYPE.MOVE:
                cell.occupant = 0;
                cell.enemy = null;
                cell = targetCell;
                cell.occupant = 1;
                cell.enemy = this;
                break;

            case MELEE_MOVE_TYPE.ATTACK:
                barbarian.Damage(5);
                break;

            case MELEE_MOVE_TYPE.INVALID:
                
                break;
        }
    }

    public void VisualTurn(MELEE_MOVE_TYPE type, int turn)
    {
        switch(type)
        {
            case MELEE_MOVE_TYPE.MOVE:
                anim.PlayAnimation("Slime_Move");
                StartCoroutine(Move(targetCell.position, 0.2f));
                StartCoroutine(Rotate(Helper.QDIR(targetCell.position - transform.position), 0.1f));
                break;

            case MELEE_MOVE_TYPE.ATTACK:
                anim.PlayAnimation("Slime_Attack");
                StartCoroutine(Rotate(Helper.QDIR(targetCell.position - transform.position), 0.1f));
                break;

            case MELEE_MOVE_TYPE.INVALID:

                break;
        }
    }

    public void CauldronVisualSpawn(Cell c)
    {
        anim.PlayAnimation("Slime_Jump");
        StartCoroutine(Move(c.position, 1.0f));
        StartCoroutine(Rotate(Helper.QDIR(c.position - transform.position), 0.2f));
    }

    private void Initialize()
    {
        map = Map.FindMap();
        barbarian = Barbarian.FindBarbarian();
        anim = GetComponent<AnimatedObject>();
        anim.Initialize();
        anim.PlayAnimation("Slime_Idle");
    }
    
    private void Update() { }



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

        FX.Emit(transform.localPosition + Game.HALF_Y, Quaternion.identity, FX.VFX.SlimeDeath, 30);
        Destroy(gameObject);

    }
}
