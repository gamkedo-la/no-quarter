using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitch : MonoBehaviour
{   
    private PlayerInputHandler playerInputHandler;
    private int selectedWeapon = 0;
    
    // Start is called before the first frame update
    void Start()
    {   
        playerInputHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInputHandler>();
        SelectWeapon();
    }

    private void OnEnable()
    {
        PlayerInputHandler.OnWeaponScroll += ScrollWeapon;
    }

    private void OnDisable() 
    {
        PlayerInputHandler.OnWeaponScroll -= ScrollWeapon;
    }

    private void SelectWeapon()
    {   
        int i=0;
        foreach (Transform weapon in transform)
        {   
            if (i == selectedWeapon )
            {
                weapon.gameObject.SetActive(true);
            }
            else 
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }

    private void ScrollWeapon(int i) {
        if (i > 0) {
            SelectNextWeapon();
        }
        else if (i < 0)
        {
            SelectPrevWeapon();
        }
    }

    private void SelectNextWeapon()
    {
        selectedWeapon++;
        //if holding last gun in list, wrap to beginning
        if (selectedWeapon >= transform.childCount)
        {
            selectedWeapon = 0;
        }
        SelectWeapon();
    }

    private void SelectPrevWeapon()
    {
        selectedWeapon--;
        //if holding first weapon in list, wrap to back
        if (selectedWeapon < 0)
        {
            selectedWeapon = transform.childCount - 1;
        }
        SelectWeapon();
    }
}
