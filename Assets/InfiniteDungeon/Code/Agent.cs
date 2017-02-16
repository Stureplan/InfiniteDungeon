using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour, IDamageable<int>
{
    public int maxHealth;
    public int health;
    public Map map;
    public Cell cell;
    public Barbarian barbarian; 

    public virtual void Damage(int damage) { }
    public virtual void Kill() { }

    public virtual void SetupEnemy(Cell c) { }
    public virtual void NextTurn(int turn) { }
}
