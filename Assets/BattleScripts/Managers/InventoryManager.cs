using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct ItemStack
{
    public Item item;
    public int count;
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    public Dictionary<Item, int> items = new Dictionary<Item, int>();
    private void Awake()
    {
        instance = this;

        // Transfer list to dictionary for fast logic
        foreach (var entry in PartyManager.instance.inventory)
        {
            items.Add(entry.item, entry.count);
        }
    }

    public void AddItem(Item item, int count)
    {
        if (items.ContainsKey(item))
            items[item] += count;
        else
            items.Add(item, count);
    }

    public void RemoveItem(Item item)
    {
        if (items.ContainsKey(item))
        {
            items[item]--;
            if (items[item] <= 0) items.Remove(item);
        }
    }

    public int GetItemCount(Item item)
    {
        return items.ContainsKey(item) ? items[item] : 0;
    }

    public void SyncToPartyManager()
    {
        PartyManager.instance.inventory.Clear();

        foreach (var pair in items)
        {
            PartyManager.instance.inventory.Add(new ItemStack 
            { 
                item = pair.Key, 
                count = pair.Value 
            });
        }
    }
}
