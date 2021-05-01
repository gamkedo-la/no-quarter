using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WeaponMods/Rapidfire", fileName = "Rapidfire")]
public class WMRapidfire : WeaponMod
{
    public override void ProjectileCollision(Collider collision, Vector3 projectileDirection)
    {
        throw new System.NotImplementedException();
    }
}
