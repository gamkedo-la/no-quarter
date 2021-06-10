using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.LWRP;

[CreateAssetMenu(menuName = "WeaponMods/Ricochet", fileName = "Ricochet")]
public class WMRicochet : WeaponMod
{
    public override bool OnCollision(RaycastHit hitInfo, Projectile projectile)
    {
        projectile.projectileDirection = Vector3.Reflect(projectile.projectileDirection, hitInfo.normal);
        return true;
    }
}
