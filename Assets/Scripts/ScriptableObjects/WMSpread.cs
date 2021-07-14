using UnityEngine;

[CreateAssetMenu(menuName = "WeaponMods/Spread", fileName = "Spread")]
public class WMSpread : WeaponMod
{
    public override bool OnCollision(RaycastHit hitInfo, Projectile projectile)
    {
        return false;
    }
}
