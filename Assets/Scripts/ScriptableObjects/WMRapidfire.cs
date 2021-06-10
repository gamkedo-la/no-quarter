using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WeaponMods/Rapidfire", fileName = "Rapidfire")]
public class WMRapidfire : WeaponMod
{
    public override bool OnCollision(RaycastHit hitInfo, Projectile projectile)
    {
        return true;
    }
}
