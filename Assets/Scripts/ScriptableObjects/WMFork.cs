using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WeaponMods/Fork", fileName = "Fork")]
public class WMFork : WeaponMod
{
    public override bool OnCollision(RaycastHit hitInfo, Projectile projectile)
    {
        return true;
    }
}