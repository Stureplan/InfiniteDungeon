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
        Blood02,
        SlimeDeath
    }

    // The object to use.
    static GameObject fx;
    static ParticleSystem ps;

    // All these need to be World space to work.
    static ParticleSystem fireball;
    static ParticleSystem frostbolt;
    static ParticleSystem blood01;
    static ParticleSystem blood02;
    static ParticleSystem slimeDeath;


    public static void Initialize()
    {
        fx = new GameObject("FX");
        fx.transform.position = new Vector3(0.0f, 1000.0f, 0.0f);
        ps = fx.AddComponent<ParticleSystem>();


        slimeDeath = (ParticleSystem)Resources.Load("FX/SlimeDeath", typeof(ParticleSystem));
    }

    public static void Emit(Vector3 point, Quaternion rotation, VFX VFX, int amount)
    {
        switch (VFX)
        {
            case VFX.Fireball:
                ps = fireball;

                break;

            case VFX.SlimeDeath:
                ps = slimeDeath;
                break;
        }

        fx.transform.localPosition = point;
        fx.transform.localRotation = rotation;
        ps.Emit(amount);
    }
}
