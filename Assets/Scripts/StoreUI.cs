using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class StoreUI : MonoBehaviour
{
    [SerializeField] private TMP_Text itemTitle;
    [SerializeField] private TMP_Text itemDescription;
    [SerializeField] private TMP_Text itemPrice;
    [SerializeField] private Transform gridLayoutGroup;
    [Space]
    [SerializeField] private ScriptableObject[] storeInventory;
    [SerializeField] private GameObject storeItemPrefab;
    
    private List<ScriptableObject> itemsOwned;
    private int selection = 0;

    void Start()
    {
        // TODO: instantiate this based on player's save game data.
        itemsOwned = new List<ScriptableObject>();

        // Remove example items from hierarchy.
        gridLayoutGroup.gameObject.DestroyAllChildren();

        // Create UI entries for each unpurchased item.
        var i = 0;
        foreach (var item in storeInventory)
        {
            if (!itemsOwned.Contains(item))
            {
                if (i == selection)
                {
                    UpdateItemInfo(item);
                }
                var storeEntry = Instantiate(storeItemPrefab, gridLayoutGroup);
                storeEntry.GetComponent<StoreItem>().SetItem(item);
                storeEntry.GetComponent<Button>().onClick.AddListener(() => SetSelection(storeEntry));
                i++;
            }
        }
    }

    void SetSelection(GameObject storeEntry)
    {
        var storeItem = storeEntry.GetComponent<StoreItem>();
        var so = storeItem.GetItem();
        UpdateItemInfo(so);
    }

    void UpdateItemInfo(ScriptableObject itemInfo)
    {
        // Handle different SOs so that we can purchase both weapons
        // and modifiers from the same storefront.
        if (itemInfo is WeaponMod)
        {
            var modInfo = (WeaponMod) itemInfo;
            itemTitle.text = modInfo.title;
            itemDescription.text = modInfo.description;
            itemPrice.text = modInfo.purchasePrice.ToString("N0", CultureInfo.CreateSpecificCulture("en-US"));
        }
    }
}
