using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour, IDamageable<int>
{
    public int maxHealth;
    public int health;
    public int value;
    public CURRENT_STATUS status = CURRENT_STATUS.NONE;
    //public bool dead;
    public Map map;
    public Cell cell;
    public Barbarian barbarian;

    public virtual void Damage(int damage) { }
    public virtual void Kill() { }
    public virtual void PushBack(Cell c) { }

    public virtual void SetupEnemy(Cell c) { }
    public virtual void NextTurn(int turn) { }

    public void HandleMoney()
    {
        FX.Emit(transform.localPosition + Vector3.up, Quaternion.identity, FX.VFX.Coins, value);
        barbarian.AddMoney(value);
    }
}