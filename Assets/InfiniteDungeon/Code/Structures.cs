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

public enum MOVE_TYPE
{
    MOVE,
    ATTACK,
    INVALID,
    FINISH
}

public enum SLIME_MOVE_TYPE
{
    MOVE,
    ATTACK,
    INVALID
}

public static class QHelp
{
    public static Quaternion QDIR(Vector3 dir)
    {
        return Quaternion.LookRotation(dir);
    }
}