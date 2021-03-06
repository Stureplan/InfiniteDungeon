﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barbarian : MonoBehaviour, IDamageable<int>
{
    public Camera cam;
    public Map map;
    public BarbarianUI ui;
    public Cell cell { get; set; }


    private Vector3 camOffset;
    private AnimatedObject anim;
    private int currentDamage = 1;
    private int currentHealth = MAX_HEALTH;
    private Agent currentAgent;
    private int currentCoins = 0;

    private int frame = 0;
    private int vegetationUpdate = 1;

    public const int MAX_HEALTH = 100;
    public static Barbarian barbarian;
    public static Barbarian FindBarbarian()
    {
        if (barbarian == null)
        {
            barbarian = (Barbarian)FindObjectOfType<Barbarian>();
        }

        return barbarian;
    }

    public void Damage(int damage)
    {
        currentHealth -= damage;
        ui.SetHealth(currentHealth);
        ui.FloatingDamageText(damage);
        if (currentHealth < 1) { Kill(); }
    }

    public void Kill()
    {

        Game.Over();
    }

    public void DamageAgents()
    {
        currentAgent.Damage(currentDamage);
    }

    public void AddMoney(int c)
    {
        currentCoins += c;
        ui.SetCoins(currentCoins);
    }
    
    public float NextTurn(int d, int turn)
    {
        Cell c;
        MOVELIST dir = (MOVELIST)d;
        BARBARIAN_MOVE_TYPE type = DecideMove(dir, out c);
        ExecuteMove(c, type);

        if (type == BARBARIAN_MOVE_TYPE.INVALID) { return 0.01f; }
        return VisualTurn(type, c, turn);
    }

    private BARBARIAN_MOVE_TYPE DecideMove(MOVELIST dir, out Cell c)
    {
        switch (dir)
        {
            case MOVELIST.M_LEFT:
                c = map.CellAtPoint(cell.x - 1, cell.y);
                break;

            case MOVELIST.M_FORWARD:
                c = map.CellAtPoint(cell.x, cell.y+1);
                break;

            case MOVELIST.M_RIGHT:
                c = map.CellAtPoint(cell.x + 1, cell.y);
                break;

            case MOVELIST.M_BACK:
                c = map.CellAtPoint(cell.x, cell.y-1);
                break;

            case MOVELIST.S_STOMP:
                c = cell;
                return BARBARIAN_MOVE_TYPE.SPELL_STOMP;

            default:
                c = cell;
                break;
        }

        if (c == null)
        {
            // Something went wrong.
            return BARBARIAN_MOVE_TYPE.INVALID;
        }

        if (c.type == 666)
        {
            // Probably out of bounds.
            return BARBARIAN_MOVE_TYPE.INVALID;
        }

        if (c.type == 5)
        {
            return BARBARIAN_MOVE_TYPE.FINISH;
        }

        if (c.occupant == 1)
        {
            // Occupied tile that (can be attacked).
            return BARBARIAN_MOVE_TYPE.ATTACK;
        }

        if ((c.type == 0 || c.type == 4) && c.occupant == 0)
        {
            // Empty ground tile.
            return BARBARIAN_MOVE_TYPE.MOVE;
        }
        else
        {
            // Some garbage.
            return BARBARIAN_MOVE_TYPE.INVALID;
        }
    }

    private void ExecuteMove(Cell c, BARBARIAN_MOVE_TYPE type)
    {
        switch(type)
        {
            case BARBARIAN_MOVE_TYPE.MOVE:
                cell.occupant = 0;
                cell = c;
                cell.occupant = 2;
                break;

            case BARBARIAN_MOVE_TYPE.ATTACK:
                //c.enemy.Damage(currentDamage);
                currentAgent = c.enemy;
                break;

            case BARBARIAN_MOVE_TYPE.SPELL_STOMP:
                //Agent[] enemies = map.Agents3x3(cell.x, cell.y);
                Agent[] enemies = Game.AllEnemies();
                for (int i = 0; i < enemies.Length; i++)
                {
                    enemies[i].PushBack(cell);
                }
                break;

            case BARBARIAN_MOVE_TYPE.INVALID:
                break;

            case BARBARIAN_MOVE_TYPE.FINISH:
                // TODO: Implement "Next Level"
                break;

            default:
                break;
        }
    }




    public float VisualTurn(BARBARIAN_MOVE_TYPE type, Cell c, int turn)
    {
        // Handle animations, effects etc.
        // Lerp to new cell.
        float r = Game.BARBARIAN_TIMER;

        switch (type)
        {
            case BARBARIAN_MOVE_TYPE.MOVE:
                anim.PlayAnimation("Barbarian_Move");
                StartCoroutine(Move(c.position, 0.5f));
                StartCoroutine(MoveCamera(c.position + camOffset, 0.5f));
                StartCoroutine(Rotate(Helper.QDIR(c.position - transform.position), 0.1f));
                break;
            case BARBARIAN_MOVE_TYPE.ATTACK:
                anim.PlayAnimation("Barbarian_Attack1");
                StartCoroutine(Rotate(Helper.QDIR(c.position - transform.position), 0.1f));
                break;

            case BARBARIAN_MOVE_TYPE.SPELL_STOMP:
                anim.PlayAnimation("Barbarian_Stomp");
                FX.Emit(transform.localPosition + (Game.HALF_Y * 0.01f), Quaternion.identity, FX.VFX.Stomp, 1);
                break;

            case BARBARIAN_MOVE_TYPE.FINISH:
                //Here goes jump down animations and map lerps
                break;

            default:
                r = 0.0f;
                break;
        }



        return r;
    }

    public void CheckSurroundings()
    {
        int[] occupants = map.SurroundingOccupants(cell.x, cell.y);
        for (int i = 0; i < occupants.Length; i++)
        {
            ui.ChangeSprite((MOVELIST)i, occupants[i]);
        }
    }



    public void InitiateLevel()
    {
        map.AnimateMap();

        // Barbarian has jumped off the first platform!
        anim.PlayAnimation("Barbarian_JumpingDown");
        StartCoroutine(Move(map.StartCell().position, 1.0f));
        StartCoroutine(MoveCamera(map.StartCell().position + camOffset, 1.0f));
    }

    public void StartLevel()
    {
        // Barbarian has landed!
        cell = map.StartCell();
        anim.PlayAnimation("Barbarian_Land");
        ui.FadePanelsIn();
        ui.SetMinimap(map.GetMinimap());
    }

    private void Start()
    {
        camOffset = cam.transform.position - transform.position;
        anim = GetComponent<AnimatedObject>();
        anim.Initialize();
        anim.PlayAnimation("Barbarian_LookingDown");


        map.Generate(); // <-- initiates the animation sequence
        //cell = map.StartCell();

        //ui.FadePanelsIn();
        //ui.SetMinimap(map.GetMinimap());
    }

    private void Update()
    {
        if (frame % vegetationUpdate == 0)
        {
            Vector4 vec = new Vector4(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z, 1.0f);
            Shader.SetGlobalVector("GLOBAL_PLAYER_POS", vec);
        }


        frame++;
    }

    private void OnTriggerEnter(Collider other)
    {
        //TODO: Set up Visual triggers.
        // Vegetation, Destructible, etc.
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
        transform.localPosition = end;
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
        transform.localRotation = end;
    }

    private IEnumerator MoveCamera(Vector3 end, float time)
    {
        float t = 0.0f;

        while (t < time)
        {
            cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, end, t / time);
            t += Time.deltaTime;

            yield return null;
        }
        cam.transform.localPosition = end;

    }
}
