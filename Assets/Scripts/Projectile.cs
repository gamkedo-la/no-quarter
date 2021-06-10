using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Projectile : TeleportAgent
{
    public float baseDamage = 1.0f;
    public float projectileSpeed;
    public float maxDistance;
    public int baseProjectileCount = 1;
    public Vector3 projectileDirection;

    [SerializeField]
    private float spinRate = 30f;

    public FPSWeapon originator;

    private float distanceTraveled;

    private void Start()
    {
        distanceTraveled = 0f;
    }

    private void Update()
    {
        var travelDistance = projectileSpeed * Time.deltaTime;
        var target = transform.position + travelDistance * projectileDirection;

        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, projectileDirection, out hitInfo, travelDistance, LayerMask.NameToLayer("PlayerProjectile")))
        {
            target = hitInfo.point;
            travelDistance = hitInfo.distance;

            var keepAlive = false;
            foreach (var mod in originator.EquippedMods)
            {
                keepAlive = keepAlive || mod.OnCollision(hitInfo, this);
            }

            hitInfo.collider.SendMessageUpwards("TakeDamage", baseDamage, SendMessageOptions.DontRequireReceiver);

            if (!keepAlive)
            {
                Destroy(gameObject);
            }
        }

        distanceTraveled += travelDistance;
        transform.position = target;

        // Spin the projectile as it flies.
        transform.RotateAround(transform.localPosition, transform.forward, spinRate * Time.deltaTime);

        if (distanceTraveled > maxDistance)
        {
            Destroy(gameObject);
        }
    }
}
