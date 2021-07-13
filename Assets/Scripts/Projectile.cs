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

    public GameObject spawnOnImpact; // optional fx

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
        // $CTK: we want bullets to hit walls too - but buggy if I remove this mask ---v
        //, LayerMask.NameToLayer("PlayerProjectile")))
        // FIXME: no such layer in edit->projectsettings->physics????
        // bah
        {
            target = hitInfo.point;
            travelDistance = hitInfo.distance;

            var keepAlive = false;
            foreach (var mod in originator.EquippedMods)
            {
                keepAlive = keepAlive || mod.OnCollision(hitInfo, this);
            }

            if (hitInfo.collider!=null)
                hitInfo.collider.SendMessageUpwards("TakeDamage", baseDamage, SendMessageOptions.DontRequireReceiver);

            if (!keepAlive)
            {
                if (spawnOnImpact!=null) {
                    Debug.Log("Projectile spawning Impact FX!");
                    Instantiate(spawnOnImpact,transform.position,transform.rotation);
                }
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
