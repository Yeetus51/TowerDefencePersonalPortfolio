using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public int damagePotential = 0;
    [HideInInspector] public float speed;
    [HideInInspector] public Vector3 ogPostion;
    const float speedConstant = 10; // Do not change 

    private void FixedUpdate()
    {
        transform.Translate(Vector3.forward * speed / speedConstant);
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, ogPostion) > 100) gameObject.SetActive(false);
    }


}
