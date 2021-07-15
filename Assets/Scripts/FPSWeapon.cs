using System;
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

    public Sprite reticle;
    public Color reticleTint;

	public List<AudioClip> gunshotSFX = new List<AudioClip>();


    private void Start()
    {
        // Get equipped mods from save file.
        var gm = GameManager.Instance;

        if (gm)
        {
            mods = new List<WeaponMod>();
            foreach (var modName in gm.saveData.equippedMods)
            {
                var mod = Resources.Load<WeaponMod>(modName);
                mods.Add(mod);
            }
        }
    }

    int GetProjectileCount()
    {
        var projectileCount = projectile.baseProjectileCount;

        foreach (var mod in mods)
        {
            projectileCount += mod.additionalProjectiles;
        }

        return projectileCount;
    }

    public List<WeaponMod> EquippedMods
    {
        get
        {
            return mods;
        }
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
            var projectileComponent = projectileInstance.GetComponent<Projectile>();
            projectileComponent.projectileDirection = dir;
            projectileComponent.originator = this;
        }
    }


}
