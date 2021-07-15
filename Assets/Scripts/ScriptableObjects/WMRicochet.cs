using UnityEngine;

[CreateAssetMenu(menuName = "WeaponMods/Ricochet", fileName = "Ricochet")]
public class WMRicochet : WeaponMod
{
    public int maxBounces = 4;

    private float horizontalSurfaceTolerance = 30f;
    private int bounces = 0;

    public override bool OnCollision(RaycastHit hitInfo, Projectile projectile)
    {
        if (++projectile.bounces > maxBounces) return false;

        Debug.Log($"used {projectile.bounces}/{maxBounces} bounces");

        projectile.projectileDirection = Vector3.Reflect(projectile.projectileDirection, hitInfo.normal);

        if (projectile.useGravity && Vector3.Angle(hitInfo.normal, Vector3.up) < horizontalSurfaceTolerance)
        {
            projectile.ReverseGravity();
        }

        return true;
    }
}
