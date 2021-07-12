using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    private string saveDataPath;

    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    public SaveData saveData;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            saveDataPath = Application.persistentDataPath + "/Save.dat";
            saveData = new SaveData();

            if (!DoesSaveFileExist()) CreateSave();
            else LoadGame();

            DontDestroyOnLoad(gameObject);
        }
    }

    public void LoadGame()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        var file = File.OpenRead(saveDataPath);
        saveData = (SaveData)binaryFormatter.Deserialize(file);
    }

    public void SaveGame()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        var file = File.OpenWrite(saveDataPath);
        binaryFormatter.Serialize(file, saveData);
        file.Close();
    }

    private void CreateSave()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream file = File.Create(saveDataPath);
        saveData = new SaveData();
        binaryFormatter.Serialize(file, saveData);
        file.Close();
    }

    private bool DoesSaveFileExist()
    {
        return File.Exists(Application.persistentDataPath + "/Save.dat");
    }

    public void ResetSaveFile()
    {
        saveData = new SaveData();
        SaveGame();
    }
}
