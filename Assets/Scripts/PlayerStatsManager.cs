using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class PlayerStatsManager : MonoBehaviour
{
    private bool paused = false;    //TODO: support game pausing -> stamina recovery halted while on Pause Screen

    public float maxHealth = 100f;
    public float lowHealthThreshold = 20f;
    public float currentHealth = 100f;
    private bool isImmortal = false;

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
        SaveData data = new SaveData();
        foreach (var item in playerInputHandler.equippedMods)
        {
            data.equippedMods.Add(item.name);
        }
        binaryFormatter.Serialize(file, data);
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
            SaveData data = (SaveData)bf.Deserialize(file);
            file.Close();

            playerInputHandler.equippedMods.Clear();
            foreach (var item in data.equippedMods)
            {
                string scriptableObjectPath = UnityEditor.AssetDatabase.GUIDToAssetPath(UnityEditor.AssetDatabase.FindAssets(item)[0]);
                WeaponMod weaponModToAdd = UnityEditor.AssetDatabase.LoadAssetAtPath<WeaponMod>(scriptableObjectPath);
                playerInputHandler.equippedMods.Add(weaponModToAdd);
            }

            Debug.Log("Game data loaded!");
        }
        else
            Debug.LogError("There is no save data!");
    }
}
