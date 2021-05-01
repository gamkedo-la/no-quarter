using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WeaponMods/Spread", fileName = "Spread")]
public class WMSpread : WeaponMod
{
    public override void ProjectileCollision(Collider collision, Vector3 projectileDirection)
    {
        throw new System.NotImplementedException();
    }
}
