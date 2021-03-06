﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowBolt : MonoBehaviour 
{
    const float TIME = 0.3f;

    Cell targetCell;
    float distance;
    Vector3 targetPos;
    Vector3 direction;

    float cDist = 0;
    float speed;


    public void Init(Cell t)
    {
        direction = transform.forward;
        targetCell = t;
        targetPos = targetCell.position + Game.HALF_Y;

        distance = Vector3.Distance(transform.position, targetPos);
        speed = distance / TIME;
    }

    private void Update()
    {
        transform.localPosition += (direction * speed * Time.deltaTime);

        cDist = Vector3.Distance(transform.localPosition, targetPos);
        if (cDist < 0.1f)
        {
            //hit

            FX.Emit(transform.position, transform.rotation, FX.VFX.ShadowBoltHit, 50);
            Barbarian.FindBarbarian().Damage(10);

            ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
            Transform tr = GetComponentInChildren<TrailRenderer>().transform;
            ParticleSystem.EmissionModule em = ps.emission;
            em.enabled = false;
            
            tr.SetParent(null, true);
            ps.transform.SetParent(tr, true);

            GetComponentInChildren<VLight>().Hide();

            Destroy(tr.gameObject, 0.5f);
            Destroy(gameObject);
        }
    }
}
