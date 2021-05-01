using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float baseDamage = 1.0f;
    public float projectileSpeed;
    public float maxDistance;
    public int baseProjectileCount = 1;
    public Vector3 projectileDirection;
    public Vector3 startPosition;

    public float DistanceTraveled
    {
        get
        {
            return Vector3.Distance(startPosition, transform.position);
        }
    }

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        transform.position += projectileSpeed * Time.deltaTime * projectileDirection;

        if (DistanceTraveled > maxDistance)
        {
            Destroy(gameObject);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        other.SendMessageUpwards("TakeDamage", baseDamage);
        Destroy(gameObject);
    }
}
