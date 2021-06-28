using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class PlayerStatsManager : MonoBehaviour
{
    private bool paused = false;    //TODO: support game pausing -> stamina recovery halted while on Pause Screen

    public static SaveData saveData;
    private List<ScriptableObject> itemsOwned;

    public float maxHealth = 100f;
    public float lowHealthThreshold = 20f;
    public float currentHealth = 100f;
    private bool isImmortal = false;
    public int currency;

    [Header("Stamina")]
    [SerializeField] private float staminaChargeRecoveryTime = 5.0f;
    private int staminaChargeCount = 3;
    private Queue<IEnumerator> staminaChargeQueue = new Queue<IEnumerator>();
    private IEnumerator currentRecharge = null;
    public static event Action<int> OnStaminaChange;

    public List<AudioClip> heartbeats = new List<AudioClip>();
    public float heartbeatPace = 1f;
    private SfxHelper sfx;

    public delegate void HealthChange(float currentHealth);
    public static event HealthChange OnHealthChange;

    private PlayerInputHandler playerInputHandler;

    private void OnEnable() {
        PlayerInputHandler.dashStarted += UseStaminaCharge;
    }

    private void OnDisable() {
        PlayerInputHandler.dashStarted -= UseStaminaCharge;
    }

    void Start()
    {
        // TODO: Rather than creating this out of the blue, check if there's a save file on disk already.
        if (saveData == null) LoadGame();

        // TODO: remove this later
        saveData.currency = 10000;

        currentHealth = maxHealth;
        sfx = GetComponent<SfxHelper>();
        StartCoroutine(PlayHeartbeat());
        StartCoroutine(StaminaChargesCoordinator());
        playerInputHandler = gameObject.GetComponent<PlayerInputHandler>();
    }

    IEnumerator PlayHeartbeat()
    {
        while (true)
        {
            var waitTime = heartbeatPace;
            if (currentHealth < lowHealthThreshold)
            {
                sfx.PlayRandomAudioOneshot(heartbeats, 0.7f, 1.0f, 0.9f, 1.1f);
            }
            yield return new WaitForSeconds(waitTime);
        }
    }

    public void TakeDamage(float amount) {
        if(isImmortal) {
            return;
        }
        if (amount > 0) {
            currentHealth -= amount;
            if (currentHealth <= 0 ) {
                //TODO: player death or level fail; currently just reset hp to maxhp
                currentHealth = maxHealth;
            }
        }
        OnHealthChange?.Invoke(currentHealth);
    }

    public void RestoreHealth(float amount)
    {
        if (amount > 0)
        {
            currentHealth += amount;
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
        }
        OnHealthChange?.Invoke(currentHealth);
    }

    
    private int CountStaminaChargesLeft()
    {
        int chargesLeft = staminaChargeCount - staminaChargeQueue.Count;
        if (currentRecharge != null) chargesLeft--;
        return chargesLeft;
    }

    private IEnumerator StaminaChargesCoordinator()
    {
        while(true)
        {
            while (staminaChargeQueue.Count > 0)
            {   
                currentRecharge = staminaChargeQueue.Dequeue();
                yield return StartCoroutine(currentRecharge);
                currentRecharge = null;
                OnStaminaChange?.Invoke(CountStaminaChargesLeft());
            }
            yield return null;
        }
    }

    private IEnumerator RecoverStaminaCharge()
    {
        float counter = 0;
        while (counter < staminaChargeRecoveryTime)
        {
            if(!paused)
            {
            counter += Time.deltaTime;
            }
            yield return null;
        }
    }

    public void UseStaminaCharge()
    {
        staminaChargeQueue.Enqueue(RecoverStaminaCharge());
        OnStaminaChange?.Invoke(CountStaminaChargesLeft());
    }

    public void SetImmortal(bool value) {
        isImmortal = value;
    }

    public void SaveGame()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/Save.dat");
        // SaveData data = new SaveData();
        //For weapon mods that are currently equipped. We store the names of the modes, then find those later in resources.
        // foreach (var item in saveData.equippedMods)
        // {
        //     saveData.equippedMods.Add(item.name);
        // }
        // saveData.currency = currency;
        //Placeholder stats
        saveData.stat1 = "";
        saveData.stat2 = "";
        saveData.stat3 = "";
        saveData.stat4 = "";
        saveData.stat5 = "";
        saveData.stat6 = "";
        saveData.stat7 = "";
        saveData.stat8 = "";
        saveData.stat9 = "";

        binaryFormatter.Serialize(file, saveData);
        file.Close();
        Debug.Log("Game saved");
    }

    public void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath
                   + "/Save.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file =
                       File.Open(Application.persistentDataPath
                       + "/Save.dat", FileMode.Open);
            // SaveData data =
            saveData = (SaveData)bf.Deserialize(file);
            file.Close();

            playerInputHandler.equippedMods.Clear();
            foreach (var item in saveData.equippedMods)
            {
                string scriptableObjectPath = UnityEditor.AssetDatabase.GUIDToAssetPath(UnityEditor.AssetDatabase.FindAssets(item)[0]);
                WeaponMod weaponModToAdd = UnityEditor.AssetDatabase.LoadAssetAtPath<WeaponMod>(scriptableObjectPath);
                itemsOwned.Add(weaponModToAdd);
                playerInputHandler.equippedMods.Add(weaponModToAdd);
            }
            saveData.currency = currency;
            //Placeholder stats
            var stat1 = saveData.stat1;
            var stat2 = saveData.stat2;
            var stat3 = saveData.stat3;
            var stat4 = saveData.stat4;
            var stat5 = saveData.stat5;
            var stat6 = saveData.stat6;
            var stat7 = saveData.stat7;
            var stat8 = saveData.stat8;
            var stat9 = saveData.stat9;

            Debug.Log("Game data loaded!");
        }
        else
        {
            InitializeSaveData();
        }
    }

    private void InitializeSaveData()
    {
        saveData = new SaveData();
        // SaveGame();
    }

    public int GetCurrency()
    {
        return saveData.currency;
    }

    public List<ScriptableObject> GetItemsOwned()
    {
        return itemsOwned;
    }

    public int UnlockMod(WeaponMod mod)
    {
        Debug.Log(mod.name);
        itemsOwned.Add(mod);
        saveData.equippedMods.Add(mod.name);
        saveData.currency -= mod.purchasePrice;
        SaveGame();

        return saveData.currency;
    }

    public int UnlockWeapon(FPSWeapon weapon)
    {
        Debug.Log(weapon.name);
        if (saveData.weapons.Contains(weapon.name))
        {
            Debug.LogErrorFormat("Weapon {0} purchased, but already owned", weapon.name);
        }
        saveData.weapons.Add(weapon.name);
        // todo: create a scriptableObject around weapons, or else figure out a different way to represent
        // them in the UI
        // saveData.currency -= weapon.purchasePrice;
        return saveData.currency;
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.transform.parent.CompareTag("Enemy"))
        {
            Transform otherParent = other.transform.parent;
            EnemyRangeAI enemy = otherParent.GetComponent<EnemyRangeAI>();
            TakeDamage(enemy.GetDamageAmount());
        }
    }
}
