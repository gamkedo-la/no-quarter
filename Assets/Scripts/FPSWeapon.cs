using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSWeapon : MonoBehaviour
{
    [SerializeField]
    private Projectile projectile;
    [SerializeField]
    private List<WeaponMod> mods;
    [SerializeField]
    private Transform firePosition;
    [SerializeField]
    private float fireDelay = 0.3f;

	public List<AudioClip> gunshotSFX = new List<AudioClip>();


    int GetProjectileCount()
    {
        var projectileCount = projectile.baseProjectileCount;

        foreach (var mod in mods)
        {
            projectileCount += mod.additionalProjectiles;
        }

        return projectileCount;
    }

    public float GetFireDelay()
    {
        var delay = fireDelay;

        foreach (var mod in mods)
        {
            delay *= mod.fireDelayMultiplier;
        }

        return delay;
    }

    List<Vector3> GetProjectileDirections(Vector3 originalDirection, Transform playerCamera)
    {
        var numProjectiles = GetProjectileCount();
        List<Vector3> directions = new List<Vector3>();

        if (numProjectiles > 1)
        {
            for (var i = 0; i < numProjectiles; i++)
            {
                Vector3 spread = UnityEngine.Random.insideUnitCircle * 0.1f;
                var skewedDirection = originalDirection + (spread.x * playerCamera.right) + (spread.y * playerCamera.up);
                directions.Add(skewedDirection);
            }
        }
        else
        {
            directions.Add(originalDirection);
        }

        return directions;
    }

    // TODO: remove need for playerCamera reference?
    public void Fire(Vector3 fireDirection, Transform playerCamera)
    {
        var fireDirections = GetProjectileDirections(fireDirection, playerCamera);

        foreach (var dir in fireDirections)
        {
            var projectileInstance = Instantiate(projectile, firePosition.position, playerCamera.rotation);
            projectileInstance.GetComponent<Projectile>().projectileDirection = dir;
        }
    }


}
