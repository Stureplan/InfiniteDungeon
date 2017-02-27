using System.Collections;
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
        targetPos = targetCell.position + new Vector3(0, 0.5f, 0);

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
            Debug.Log("Shadow Bolt Hit");
        }
    }
}
