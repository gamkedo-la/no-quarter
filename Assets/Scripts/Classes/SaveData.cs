using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public List<string> equippedMods = new List<string>();

    public List<string> weapons = new List<string>();

    // mods applied to each weapon (?)
    // public Dictionary<int, string> weaponMods = new Dictionary<int, string>();

    public int currency;
    public string stat1;
    public string stat2;
    public string stat3;
    public string stat4;
    public string stat5;
    public string stat6;
    public string stat7;
    public string stat8;
    public string stat9;
}
