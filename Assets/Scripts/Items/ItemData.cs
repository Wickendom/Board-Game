using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemData : ScriptableObject
{
    public string itemName;
    public string ID;

    public bool isStackable = true;
}
