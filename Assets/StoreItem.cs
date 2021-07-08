using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreItem : MonoBehaviour
{
    [SerializeField]
    private Image icon;
    [SerializeField]
    private Button button;
    
    private ScriptableObject item;
    

    public void SetItem(ScriptableObject itemObject)
    {
        if (itemObject is WeaponMod)
        {
            var mod = (WeaponMod) itemObject;
            item = mod;
            icon.sprite = mod.icon;
            icon.color = mod.iconColor;
        }
    }

    public ScriptableObject GetItem()
    {
        return item;
    }
}
