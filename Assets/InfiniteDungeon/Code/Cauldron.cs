using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cauldron : Agent 
{
    private Animation animations;
    Cell targetCell;

    private int spawnTimer = 3;
    private Slime lastSlime;

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

        PlayAnimation("Cauldron_Death");
        StartCoroutine(DelayedDestruction(animations.GetClip("Cauldron_Death").length));
    }

    public override void SetupEnemy(Cell c)
    {
        c.enemy = this;
        c.occupant = 1;
        cell = c;
    }

    public override void NextTurn(int turn)
    {
        BUILDING_MOVE_TYPE type = DecideMove(turn);
        ExecuteMove(type);
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
                lastSlime = map.SpawnSlime(cell);
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
                PlayAnimation("Cauldron_Spawn");
                //FX.Emit(transform.position, Helper.QDIR(Vector3.up), FX.VFX.SlimeSpawn, 5);
                lastSlime.CauldronSpawn(targetCell);
                break;

            case BUILDING_MOVE_TYPE.INVALID:
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
        PlayAnimation("Cauldron_Idle");
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


