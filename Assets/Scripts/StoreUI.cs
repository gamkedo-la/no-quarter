using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class StoreUI : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private TMP_Text itemTitle;
    [SerializeField] private TMP_Text itemDescription;
    [SerializeField] private TMP_Text itemPrice;
    [SerializeField] private Transform gridLayoutGroup;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private TMP_Text playerCurrency;
    [SerializeField] private Button exitButton;
    [Space]
    [Header("Prefabs")]
    [SerializeField] private GameObject storeItemPrefab;
    [SerializeField] private ScriptableObject[] storeInventory;

    private List<ScriptableObject> itemsOwned;
    private string selection = "";

    private PlayerStatsManager playerStatsManager;

    void Start()
    {
        // TODO: instantiate this based on player's save game data.
        var saveData = GameManager.Instance.saveData;

        // Remove example items from hierarchy.
        gridLayoutGroup.gameObject.DestroyAllChildren();

        // Assign dynamic component references.
        playerStatsManager = FindObjectOfType<PlayerStatsManager>();
        UpdatePlayerCurrency(saveData.currency);

        itemsOwned = playerStatsManager.GetItemsOwned();
        foreach (var item in itemsOwned)
        {
            Debug.Log($"owned: {item.name}");
        }
        // itemsOwned = new List<ScriptableObject>();
        // itemsOwned =

        // Register purchase button event.
        purchaseButton.onClick.AddListener(() => PurchaseItem());

        exitButton.onClick.AddListener(() =>
        {
            SceneManager.UnloadSceneAsync("Scenes/Store");
        });

        if (playerStatsManager == null)
        {
            Debug.LogError("No player stats manager found, StoreUI cannot function correctly");
        }

        // Create UI entries for each unpurchased item.
        for (var i = 0; i < storeInventory.Length; i++)
        {
            var item = storeInventory[i];
            if (!itemsOwned.Contains(item))
            {
                var storeEntry = Instantiate(storeItemPrefab, gridLayoutGroup);
                var storeItem = storeEntry.GetComponent<StoreItem>();
                storeItem.SetItem(item);
                storeEntry.GetComponent<Button>().onClick.AddListener(() => SetSelection(storeEntry));
                storeEntry.name = storeItem.GetItem().name;
            }
        }

        // Set initially selected element for event system.
        var initialSelection = gridLayoutGroup.childCount > 0
            ? gridLayoutGroup.GetChild(0).gameObject
            : exitButton.gameObject;
        EventSystem.current.SetSelectedGameObject(initialSelection);
    }

    void SetSelection(GameObject storeEntry)
    {
        if (storeEntry != null)
        {
            var storeItem = storeEntry.GetComponent<StoreItem>();
            var so = storeItem.GetItem();
            selection = storeEntry.name;
            UpdateItemInfo(so);
        }
        else
        {
            selection = null;
            UpdateItemInfo(null);
        }
    }

    void UpdateItemInfo(ScriptableObject itemInfo)
    {
        // Handle different SOs so that we can purchase both weapons
        // and modifiers from the same storefront.
        if (itemInfo == null)
        {
            itemTitle.text = "";
            itemDescription.text = "";
            itemPrice.text = "";
        }
        else if (itemInfo is WeaponMod)
        {
            var modInfo = (WeaponMod) itemInfo;
            itemTitle.text = modInfo.title;
            itemDescription.text = modInfo.description;
            itemPrice.text = modInfo.purchasePrice.ToString("N0", CultureInfo.CreateSpecificCulture("en-US"));
        }
    }

    void UpdatePlayerCurrency(int currency)
    {
        playerCurrency.text = currency.ToString("N0");
    }

    void PurchaseItem()
    {
        var storeEntry = gridLayoutGroup.Find(selection);
        if (storeEntry != null)
        {
            var storeItem = storeEntry.GetComponent<StoreItem>();
            var so = storeItem.GetItem();

            if (so is WeaponMod)
            {
                var gm = GameManager.Instance;
                var wm = (WeaponMod) so;
                if (gm.saveData.currency >= wm.purchasePrice)
                {
                    var currency = playerStatsManager.UnlockMod(wm);
                    itemsOwned.Add(so);
                    UpdatePlayerCurrency(currency);
                    Destroy(storeEntry.gameObject);
                    SetSelection(null);
                }
            }
        }
    }
}
