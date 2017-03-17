using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FX
{
    public enum VFX
    {
        SlimeDeath = 0,
        ShadowBoltHit,
        Bats,
        Coins,
        Stomp
    }

    struct PS
    {
        public GameObject go;
        public ParticleSystem ps;
        public bool loaded;
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
    static PS bats;
    static PS coins;
    static PS stomp;


    static PROJECTILE shadowBolt = new PROJECTILE(null, false);

    public static void Initialize()
    {
        //TODO: Transition into a "load once when first used, never destroy"-pattern.
        //Important for mobile.
        slimeDeath.go = Resources.Load<GameObject>("FX/SlimeDeath");
        slimeDeath.go.transform.position = Game.HIDDEN;
        slimeDeath.loaded = false;

        shadowBoltHit.go = Resources.Load<GameObject>("FX/ShadowBoltHit");
        shadowBoltHit.go.transform.position = Game.HIDDEN;
        shadowBoltHit.loaded = false;

        

        bats.loaded = false;
    }

    public static void Emit(Vector3 point, Quaternion rotation, VFX VFX, int amount)
    {
        switch (VFX)
        {
            case VFX.SlimeDeath:
                // Hasn't been spawned yet.
                if (slimeDeath.loaded == false)
                {
                    // Spawn and assign.
                    //TODO: Move "Resources.Load" into here ---V
                    slimeDeath.go = GameObject.Instantiate(slimeDeath.go);
                    slimeDeath.ps = slimeDeath.go.GetComponent<ParticleSystem>();
                    slimeDeath.loaded = true;
                }

                slimeDeath.go.transform.localPosition = point;
                slimeDeath.go.transform.localRotation = rotation;
                slimeDeath.ps.Emit(amount);

                break;

            case VFX.ShadowBoltHit:
                // Hasn't been spawned yet.
                if (shadowBoltHit.loaded == false)
                {
                    // Spawn and assign.

                    shadowBoltHit.go = GameObject.Instantiate(shadowBoltHit.go);
                    shadowBoltHit.ps = shadowBoltHit.go.GetComponentInChildren<ParticleSystem>();
                    shadowBoltHit.loaded = true;
                }

                shadowBoltHit.go.transform.localPosition = point;
                shadowBoltHit.go.transform.localRotation = rotation;
                shadowBoltHit.ps.Emit(amount);

                break;

            case VFX.Bats:
                if (bats.loaded == false)
                {
                    bats.go = Resources.Load<GameObject>("FX/Bats");
                    bats.go = GameObject.Instantiate(bats.go);
                    bats.ps = bats.go.GetComponent<ParticleSystem>();
                    bats.loaded = true;
                }

                bats.go.transform.localPosition = point;
                bats.go.transform.localRotation = rotation;
                bats.ps.Play();

                break;

            case VFX.Coins:
                if (coins.loaded == false)
                {
                    coins.go = Resources.Load<GameObject>("FX/Coins");
                    coins.go = GameObject.Instantiate(coins.go);
                    coins.ps = coins.go.GetComponent<ParticleSystem>();
                    coins.loaded = true;
                }

                coins.go.transform.localPosition = point;
                coins.go.transform.localRotation = rotation;
                coins.ps.Emit(amount);

                break;

            case VFX.Stomp:
                if (stomp.loaded == false)
                {
                    stomp.go = Resources.Load<GameObject>("FX/StompParticle");
                    stomp.go = GameObject.Instantiate(stomp.go);
                    stomp.ps = stomp.go.GetComponent<ParticleSystem>();
                    stomp.loaded = true;
                }

                stomp.go.transform.localPosition = point;
                stomp.go.transform.localRotation = rotation;
                stomp.ps.Play();

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
