using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cauldron : Agent 
{
    private AnimatedObject anim;
    Cell targetCell;

    private int spawnTimer = 6;

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

        anim.PlayAnimation("Cauldron_Death");
        StartCoroutine(DelayedDestruction(anim.GetClipLength("Cauldron_Death")));
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
        BUILDING_MOVE_TYPE type = DecideMove(turn);
        ExecuteMove(type);
        VisualTurn(type);
    }

    public BUILDING_MOVE_TYPE DecideMove(int turn)
    {
        if (turn % spawnTimer == 0)
        {
            Cell temp = map.FirstFreeNeighbor(cell.x, cell.y);

            if (temp.type != 666)
            {
                targetCell = temp;
                return BUILDING_MOVE_TYPE.SPAWN;
            }
        }

        return BUILDING_MOVE_TYPE.NOTHING;
    }

    private void ExecuteMove(BUILDING_MOVE_TYPE type)
    {
        switch(type)
        {
            case BUILDING_MOVE_TYPE.NOTHING:
                break;

            case BUILDING_MOVE_TYPE.SPAWN:
                Game.CauldronAddSlime(cell, targetCell);
                break;

            case BUILDING_MOVE_TYPE.INVALID:
                break;
        }
    }

    private void VisualTurn(BUILDING_MOVE_TYPE type)
    {
        switch(type)
        {
            case BUILDING_MOVE_TYPE.NOTHING:
                break;

            case BUILDING_MOVE_TYPE.SPAWN:
                anim.PlayAnimation("Cauldron_Spawn");
                //FX.Emit(transform.position, Helper.QDIR(Vector3.up), FX.VFX.SlimeSpawn, 5);
                break;

            case BUILDING_MOVE_TYPE.INVALID:
                break;
        }
    }

    private void Initialize()
    {
        map = Map.FindMap();
        barbarian = Barbarian.FindBarbarian();
        anim = GetComponent<AnimatedObject>();
        anim.Initialize();
        anim.PlayAnimation("Cauldron_Idle");

        value = 8;
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


