using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barbarian : MonoBehaviour, IDamageable<int>
{
    public Map map;
    public BarbarianUI ui;
    public Cell cell { get; set; }


    private Animation animations;
    private int currentDamage = 1;
    private int currentHealth = 100;

    public void Damage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 1) { Game.Over(); }
    }
    
    public float NextTurn(int d, int turn)
    {
        Cell c;
        MOVE_DIR dir = (MOVE_DIR)d;
        MOVE_TYPE type = DecideMove(dir, out c);
        ExecuteMove(c, type);

        return VisualTurn(type, c, turn);
    }

    private MOVE_TYPE DecideMove(MOVE_DIR dir, out Cell c)
    {
        switch (dir)
        {
            case MOVE_DIR.LEFT:
                c = map.CellAtPoint(cell.x - 1, cell.y);
                break;

            case MOVE_DIR.FORWARD:
                c = map.CellAtPoint(cell.x, cell.y+1);
                break;

            case MOVE_DIR.RIGHT:
                c = map.CellAtPoint(cell.x + 1, cell.y);
                break;

            case MOVE_DIR.BACK:
                c = map.CellAtPoint(cell.x, cell.y-1);
                break;

            default:
                c = cell;
                break;
        }

        if (c == null)
        {
            // Probably outside the map bounds.
            return MOVE_TYPE.INVALID;
        }

        if (c.type == 5)
        {
            return MOVE_TYPE.FINISH;
        }

        if (c.occupant == 1)
        {
            // Occupied tile that (can be attacked).
            return MOVE_TYPE.ATTACK;
        }

        if ((c.type == 0 || c.type == 4) && c.occupant == 0)
        {
            // Empty ground tile.
            return MOVE_TYPE.MOVE;
        }
        else
        {
            // Some garbage.
            return MOVE_TYPE.INVALID;
        }
    }

    private void ExecuteMove(Cell c, MOVE_TYPE type)
    {
        switch(type)
        {
            case MOVE_TYPE.MOVE:
                cell.occupant = 0;
                cell = c;
                cell.occupant = 2;
                break;

            case MOVE_TYPE.ATTACK:
                c.enemy.Damage(currentDamage);
                break;

            case MOVE_TYPE.INVALID:
                break;

            case MOVE_TYPE.FINISH:
                // TODO: Implement "Next Level"
                break;

            default:
                break;
        }
    }




    public float VisualTurn(MOVE_TYPE type, Cell c, int turn)
    {
        // Handle animations, effects etc.
        // Lerp to new cell.
        switch (type)
        {
            case MOVE_TYPE.MOVE:
                PlayAnimation("Barbarian_Move");
                StartCoroutine(Move(c.position, 0.5f));
                StartCoroutine(Rotate(QHelp.QDIR(c.position - transform.position), 0.1f));
                break;
            case MOVE_TYPE.ATTACK:
                break;

            case MOVE_TYPE.FINISH:
                //Here goes jump down animations and map lerps
                break;

            default:
                break;
        }



        return 1.0f;
    }

    private void PlayAnimation(string anim)
    {
        animations.Stop();
        animations.Play(anim);
    }

    private void Start()
    {
        animations = GetComponent<Animation>();
        cell = map.StartCell();

        ui.FadePanelsIn();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ui.FadePanelsOut();
        }
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
