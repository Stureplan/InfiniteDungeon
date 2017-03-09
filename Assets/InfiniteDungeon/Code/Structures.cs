using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable<T>
{
    void Damage(int damage);
    void Kill();
}

public enum MOVE_DIR
{
    LEFT    = 0,
    FORWARD = 1,
    RIGHT   = 2,
    BACK    = 3,
}

// Player moves
public enum BARBARIAN_MOVE_TYPE
{
    MOVE,
    ATTACK,
    INVALID,
    FINISH
}

// Generic melee mover (Slime, etc)
public enum MELEE_MOVE_TYPE
{
    MOVE,
    ATTACK,
    INVALID
}

public enum BUILDING_MOVE_TYPE
{
    NOTHING,
    SPAWN,
    INVALID
}

public enum WARLOCK_MOVE_TYPE
{
    MOVE,
    ATTACK,
    SPELL1,
    INVALID
}

public struct VISUAL_TILE_STATS
{
	int neighbors;
	Color color;
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