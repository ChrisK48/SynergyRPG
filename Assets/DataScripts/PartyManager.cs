using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemStack
{
    public Item item;
    public int count;
}

public class PartyManager : MonoBehaviour
{
    public List<PlayerCharData> activePartyMembers;
    public int heldMoney;
    public List<ItemStack> inventory;
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
        ItemStack existingStack = inventory.Find(stack => stack.item == newItem);
        if (existingStack != null) existingStack.count++;
        else
        {
            inventory.Add(new ItemStack { item = newItem, count = 1 });
        }
    }

    public void LoseItem(Item item)
    {
        ItemStack existingStack = inventory.Find(stack => stack.item == item);
        if (existingStack != null)        {
            existingStack.count--;
            if (existingStack.count <= 0)
            {
                inventory.Remove(existingStack);
            }
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
