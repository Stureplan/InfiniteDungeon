using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable<T>
{
    void Damage(int damage);
    void Kill();
}

public enum MOVELIST
{
    M_LEFT    = 0,
    M_FORWARD = 1,
    M_RIGHT   = 2,
    M_BACK    = 3,

    S_STOMP   = 4
}

// Player moves
public enum BARBARIAN_MOVE_TYPE
{
    MOVE,
    ATTACK,
    SPELL_STOMP,
    INVALID,
    FINISH
}

// Generic melee mover (Slime, etc)
public enum MELEE_MOVE_TYPE
{
    MOVE,
    ATTACK,
    NOTHING
}

public enum BUILDING_MOVE_TYPE
{
    SPAWN,
    NOTHING,
    INVALID
}

public enum WARLOCK_MOVE_TYPE
{
    MOVE,
    ATTACK,
    SPELL1,
    NOTHING,
    INVALID
}

public enum CURRENT_STATUS
{
    STUNNED,
    NONE
}

public static class QHelp
{
}

public static class Helper
{
    public static Quaternion QDIR(Vector3 dir)
    {
        return Quaternion.LookRotation(dir);
    }

}