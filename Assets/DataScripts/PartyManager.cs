using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryEntry
{
    public Item item; // The ScriptableObject (Template)
    public int count; // Used for Consumables
    
    // This holds the "Live" data for things like Tiles
    // It's null for regular items like Potions
    public TileMetadata tileMetadata; 

    public bool IsEquipped => tileMetadata != null && tileMetadata.isEquipped;

    // Constructor for Consumables
    public InventoryEntry(Item item, int count)
    {
        this.item = item;
        this.count = count;
        this.tileMetadata = null;
    }

    // Constructor for Tiles
    public InventoryEntry(TileItem tile)
    {
        item = tile;
        count = 1;
        tileMetadata = new TileMetadata();
    }
}

[System.Serializable]
public class TileMetadata
{
    public bool isEquipped;
    // You can add more later: e.g., currentLevel, experience, etc.
}

public class PartyManager : MonoBehaviour
{
    public List<PlayerCharData> activePartyMembers;
    public int heldMoney;
    public List<InventoryEntry> inventory;
    public static PartyManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void GainItem(Item newItem)
    {
        if (newItem is TileItem tile)
        {
            // Tiles never stack, always add a new unique entry
            inventory.Add(new InventoryEntry(tile));
        }
        else
        {
            // Consumables stack by count
            InventoryEntry existing = inventory.Find(e => e.item == newItem);
            if (existing != null) existing.count++;
            else inventory.Add(new InventoryEntry(newItem, 1));
        }
    }

    public void LoseItem(Item item)
    {
        int index = inventory.FindIndex(stack => stack.item == item);

        if (index != -1)
        {
            InventoryEntry stack = inventory[index];
            stack.count -= 1;

            if (stack.count <= 0)
            {
                inventory.RemoveAt(index);
            }
            else
            {
                inventory[index] = stack; 
            }
        }
    }

    public List<InventoryEntry> GetHeldTiles()
    {
        List<InventoryEntry> obtainedTiles = inventory.FindAll(stack => stack.item is TileItem);
        return obtainedTiles;
    }

    public void SetTileEquipStatus(TileItem tileSO, bool status)
    {
        InventoryEntry entry = inventory.Find(e => e.item == tileSO && e.tileMetadata != null);
        
        if (entry != null)
        {
            entry.tileMetadata.isEquipped = status;
        }
    }

    public void GainMoney(int amount)
    {
        heldMoney += amount;
    }

    public void LoseMoney(int amount)
    {
        heldMoney = Mathf.Max(0, heldMoney - amount);
    }
}
