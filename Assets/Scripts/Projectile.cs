using System;
using UnityEngine;

public class Projectile : TeleportAgent
{
    public float baseDamage = 1.0f;
    public float projectileSpeed;
    public float maxDistance;
    public int baseProjectileCount = 1;
    public Vector3 projectileDirection;
    public Vector3 gravityDirection = Vector3.zero;
    public bool useGravity = false;
    public float gravity = 0f;
    public float maxFallRate = 100f;
    public float bounciness = 0.7f;
    public LayerMask collisionMask;
    private float fallRate;

    public GameObject spawnOnImpact; // optional fx

    [SerializeField]
    private float spinRate = 30f;

    [NonSerialized]
    public FPSWeapon originator;
    [NonSerialized]
    public int bounces = 0;

    private float distanceTraveled;

    private void Start()
    {
        distanceTraveled = 0f;
        fallRate = 0f;
    }

    private void Update()
    {
        var gravityAffect = Vector3.zero;
        if (useGravity)
        {
            fallRate = Mathf.Min(maxFallRate, fallRate + gravity * Time.deltaTime);
            gravityAffect = gravityDirection * fallRate;
        }

        var travelDistance = projectileSpeed * Time.deltaTime;
        var movement = travelDistance * projectileDirection + gravityAffect;
        var target = transform.position + movement;

        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, movement.normalized, out hitInfo, movement.magnitude, collisionMask))
        // $CTK: we want bullets to hit walls too - but buggy if I remove this mask ---v
        //, LayerMask.NameToLayer("PlayerProjectile")))
        // FIXME: no such layer in edit->projectsettings->physics????
        // bah
        {
            target = hitInfo.point;
            travelDistance = hitInfo.distance;

            if (spawnOnImpact!=null) {
                var impactfx = Instantiate(spawnOnImpact,transform.position,transform.rotation);
                Destroy(impactfx.gameObject, 2);
            }

            var keepAlive = false;
            foreach (var mod in originator.EquippedMods)
            {
                keepAlive = keepAlive || mod.OnCollision(hitInfo, this);
            }

            if (hitInfo.collider!=null)
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

    public void ReverseGravity()
    {
        fallRate = -fallRate * bounciness;
    }
}
