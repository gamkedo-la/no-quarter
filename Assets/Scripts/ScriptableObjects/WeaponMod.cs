using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class WeaponMod : ScriptableObject
{
    public string title;
    [TextArea]
    public string description;
    public Sprite icon;

    public float damageMultiplier = 1.0f;
    public int additionalProjectiles = 0;
    public float fireDelayMultiplier = 1.0f;
    public float projectileDistanceMultiplier = 1.0f;


    /// <summary>
    /// Collision handler specific to each weapon mod.
    /// </summary>
    /// <param name="hitInfo"></param>
    /// <param name="projectile"></param>
    /// <returns>Keep projectile alive?</returns>
    public abstract bool OnCollision(RaycastHit hitInfo, Projectile projectile);
}
