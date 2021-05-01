using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WeaponMods/Fork", fileName = "Fork")]
public class WMFork : WeaponMod
{
    public override void ProjectileCollision(Collider collision, Vector3 projectileDirection)
    {
        throw new System.NotImplementedException();
    }
}