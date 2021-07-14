using UnityEngine;
using UnityEngine.UI;

public class ReticleUI : MonoBehaviour
{
    [SerializeField]
    private Image reticleImage;

    void Start()
    {
        WeaponSwitch.OnWeaponSelect += ChangeWeaponReticle;
    }

    private void ChangeWeaponReticle(GameObject activeWeapon)
    {
        if (reticleImage)
        {
            var weapon = activeWeapon.GetComponent<FPSWeapon>();
            if (weapon)
            {
                reticleImage.sprite = weapon.reticle;
                reticleImage.color = weapon.reticleTint;
            }
        }
    }
}
