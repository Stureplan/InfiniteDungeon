using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour, IDamageable<int>
{
    public virtual void Damage(int damage) { }
	public virtual void NextTurn(Map map, int turn) { }
    public virtual void DecideMove(Map map, int turn) { }
    public virtual void ExecuteMove(Map map, int turn) { }
    public virtual void VisualTurn(Map map, int turn) { }
}
