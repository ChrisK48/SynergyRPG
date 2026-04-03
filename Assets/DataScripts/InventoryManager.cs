using UnityEngine;
using System.Collections.Generic;
using System;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    public Dictionary<ConsumableItem, int> items = new Dictionary<ConsumableItem, int>();

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        foreach (InventoryEntry entry in PartyManager.instance.inventory)
        {
            if (entry.item is ConsumableItem consumable && entry.count > 0)
            {
                items.Add(consumable, entry.count);
            }
        }
    }

    public void AddItem(ConsumableItem item, int count)
    {
        if (items.ContainsKey(item))
            items[item] += count;
        else
            items.Add(item, count);
    }

    public void RemoveItem(ConsumableItem item)
    {
        if (items.ContainsKey(item))
        {
            items[item]--;
            if (items[item] <= 0) items.Remove(item);
        }
    }

    public int GetItemCount(ConsumableItem item)
    {
        return items.ContainsKey(item) ? items[item] : 0;
    }

    public void SyncToPartyManager()
    {
        PartyManager.instance.inventory.Clear();

        foreach (var pair in items)
        {
            PartyManager.instance.inventory.Add(new InventoryEntry(pair.Key, pair.Value));
        }
    }
}
