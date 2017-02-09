using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FX
{
    public enum VFX
    {
        Fireball = 0,
        Frostbolt,
        Blood01,
        Blood02
    }

    // All these need to be World space to work.
    static ParticleSystem fireball;
    static ParticleSystem frostbolt;
    static ParticleSystem blood01;
    static ParticleSystem blood02;


    public static void Emit(Vector3 point, Quaternion rotation, VFX VFX, int amount)
    {
        switch (VFX)
        {
            case VFX.Fireball:
                fireball.transform.localPosition = point;
                fireball.transform.localRotation = rotation;
                fireball.Emit(amount);

                break;
        }
    }
}
