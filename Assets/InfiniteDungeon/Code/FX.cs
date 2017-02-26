using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FX
{
    public enum VFX
    {
        SlimeDeath = 0
    }

    struct PS
    {
        public GameObject go;
        public ParticleSystem ps;
        public bool spawned;
    }

    struct PROJECTILE
    {
        public PROJECTILE(GameObject go, bool s)
        {
            prefab = go;
            spawned = s;
        }

        public GameObject prefab;
        public bool spawned;
    }

    // All these need to be World space to work.
    static PS slimeDeath;

    static PROJECTILE shadowBolt = new PROJECTILE(null, false);

    static float hiddenHeight = -1000.0f;

    public static void Initialize()
    {
        //TODO: Transition into a "load once when first used, never destroy"-pattern.
        //Important for mobile.
        slimeDeath.go = Resources.Load<GameObject>("FX/SlimeDeath");
        slimeDeath.go.transform.position = new Vector3(0.0f, hiddenHeight, 0.0f);
        slimeDeath.ps = slimeDeath.go.GetComponent<ParticleSystem>();
        slimeDeath.spawned = false;
    }

    public static void Emit(Vector3 point, Quaternion rotation, VFX VFX, int amount)
    {
        switch (VFX)
        {
            case VFX.SlimeDeath:
                // Hasn't been spawned yet.
                if (slimeDeath.spawned == false)
                {
                    // Spawn and assign.
                    slimeDeath.go = GameObject.Instantiate(slimeDeath.go);
                    slimeDeath.spawned = true;
                }

                slimeDeath.go.transform.localPosition = point;
                slimeDeath.go.transform.localRotation = rotation;
                slimeDeath.ps.Emit(amount);
                break;
        }
    }

    public static void EmitNumber(Vector3 point, Color c, int number)
    {

    }

    public static GameObject ShadowBolt(Vector3 point, Quaternion rot)
    {        
        if (!shadowBolt.spawned)
        {
            shadowBolt.prefab = Resources.Load<GameObject>("FX/ShadowBolt");
            shadowBolt.spawned = true;
        }

        GameObject go = GameObject.Instantiate(shadowBolt.prefab);
        go.name = "ShadowBolt";
        go.transform.position = point;
        go.transform.rotation = rot;

        return go;
    }
}
