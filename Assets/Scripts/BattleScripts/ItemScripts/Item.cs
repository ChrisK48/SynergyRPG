using UnityEngine;

public enum ItemType { All, Consumable, Equippable, Gem, Key }
public abstract class Item : ScriptableObject
{
    public string ItemName;
    public string ItemDescription;
    [HideInInspector] public ItemType itemType;
}

