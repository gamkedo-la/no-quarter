using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponMod : ScriptableObject
{
    public float damageMultiplier = 1.0f;
    public int additionalProjectiles = 0;
    public float fireDelayMultiplier = 1.0f;
    public float projectileDistanceMultiplier = 1.0f;

    public abstract void ProjectileCollision(Collider collision, Vector3 projectileDirection);
}
