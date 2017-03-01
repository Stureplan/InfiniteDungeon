using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FX
{
    public enum VFX
    {
        SlimeDeath = 0,
        ShadowBoltHit
    }

    struct PS
    {
        public GameObject go;
        public ParticleSystem ps;
        public bool spawned;
    }

    struct PROJECTILE
    {
        public PROJECTILE(GameObject go, bool l)
        {
            prefab = go;
            loaded = l;
        }

        public GameObject prefab;
        public bool loaded;
    }

    // All these need to be World space to work.
    static PS slimeDeath;
    static PS shadowBoltHit;


    static PROJECTILE shadowBolt = new PROJECTILE(null, false);

    public static void Initialize()
    {
        //TODO: Transition into a "load once when first used, never destroy"-pattern.
        //Important for mobile.
        slimeDeath.go = Resources.Load<GameObject>("FX/SlimeDeath");
        slimeDeath.go.transform.position = Game.HIDDEN;
        slimeDeath.spawned = false;

        shadowBoltHit.go = Resources.Load<GameObject>("FX/ShadowBoltHit");
        shadowBoltHit.go.transform.position = Game.HIDDEN;
        shadowBoltHit.spawned = false;
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
                    //TODO: Move "Resources.Load" into here ---V
                    slimeDeath.go = GameObject.Instantiate(slimeDeath.go);
                    slimeDeath.ps = slimeDeath.go.GetComponent<ParticleSystem>();
                    slimeDeath.spawned = true;
                }

                slimeDeath.go.transform.localPosition = point;
                slimeDeath.go.transform.localRotation = rotation;
                slimeDeath.ps.Emit(amount);

                break;

            case VFX.ShadowBoltHit:
                // Hasn't been spawned yet.
                if (shadowBoltHit.spawned == false)
                {
                    // Spawn and assign.
                    shadowBoltHit.go = GameObject.Instantiate(shadowBoltHit.go);
                    shadowBoltHit.ps = shadowBoltHit.go.GetComponentInChildren<ParticleSystem>();
                    shadowBoltHit.spawned = true;
                }

                shadowBoltHit.go.transform.localPosition = point;
                shadowBoltHit.go.transform.localRotation = rotation;
                shadowBoltHit.ps.Emit(amount);

                break;
        }
    }

    public static void EmitNumber(Vector3 point, Color c, int number)
    {

    }

    public static GameObject ShadowBolt(Vector3 point, Quaternion rot)
    {        
        if (!shadowBolt.loaded)
        {
            shadowBolt.prefab = Resources.Load<GameObject>("FX/ShadowBolt");
            shadowBolt.loaded = true;
        }

        GameObject go = GameObject.Instantiate(shadowBolt.prefab);
        go.name = "ShadowBolt";
        go.transform.position = point;
        go.transform.rotation = rot;

        return go;
    }
}
