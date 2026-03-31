using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ItemStack
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

    public void GainItem(Item item)
    {
        int index = inventory.FindIndex(stack => stack.item == item);

        if (index == -1)
        {
            inventory.Add(new ItemStack { item = item, count = 1 });
        }
        else
        {
            ItemStack stack = inventory[index];
            stack.count += 1;
            inventory[index] = stack; 
        }
    }

    public void LoseItem(Item item)
    {
        int index = inventory.FindIndex(stack => stack.item == item);

        if (index != -1)
        {
            ItemStack stack = inventory[index];
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

    public void GainMoney(int amount)
    {
        heldMoney += amount;
    }

    public void LoseMoney(int amount)
    {
        heldMoney = Mathf.Max(0, heldMoney - amount);
    }
}
