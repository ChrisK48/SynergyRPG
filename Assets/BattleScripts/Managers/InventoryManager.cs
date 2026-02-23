using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    [System.Serializable]
    public struct StartingItem
    {
        public Item item;
        public int count;
    }
    public List<StartingItem> startingInventory;

    public Dictionary<Item, int> items = new Dictionary<Item, int>();
    private void Awake()
    {
        instance = this;

        // Transfer list to dictionary for fast logic
        foreach (var entry in startingInventory)
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
}
