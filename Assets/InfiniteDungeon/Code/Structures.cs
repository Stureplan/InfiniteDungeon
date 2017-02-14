using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable<T>
{
    void Damage(int damage);
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

public static class QHelp
{
    public static Quaternion QDIR(Vector3 dir)
    {
        return Quaternion.LookRotation(dir);
    }
}