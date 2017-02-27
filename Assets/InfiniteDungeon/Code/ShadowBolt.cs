using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowBolt : MonoBehaviour 
{
    Cell targetCell;
    float range;
    float speed;
    Vector3 direction;

	public void Init(Cell t, float r)
    {
        targetCell = t;
        range = r;

        speed = Vector3.Distance(transform.position, t.position);
        direction = transform.forward;
    }

    private void Update()
    {
        transform.localPosition += (direction * speed * Time.deltaTime);
    }
}
