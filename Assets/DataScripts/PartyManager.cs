using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using System.Linq;

[Serializable]
public class ItemStack
{
    public Item item;
    public int count;
}

public class PartyManager : MonoBehaviour
{
    public List<PlayerCharData> unlockedPartyMembers;
    public List<PlayerCharData> activePartyMembers;
    public int heldMoney;
    public List<ItemStack> inventory;
    public static PartyManager instance;
    private List<DualSynergyResult> activeDualSynergies = new List<DualSynergyResult>();
    private List<TriSynergyResult> activeTriSynergies = new List<TriSynergyResult>();
    private SynergySearchLogic synergySearchLogic;

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

    public void UpdateSynergies()
    {
        if (synergySearchLogic == null) synergySearchLogic = new SynergySearchLogic();
        activeDualSynergies = synergySearchLogic.GetDoubleSynergies(activePartyMembers);
        activeTriSynergies = synergySearchLogic.GetTripleSynergies(activePartyMembers);

        foreach (var synergy in activeDualSynergies) Debug.Log($"Active Dual Synergy: {synergy.Ability.name} between {synergy.UserA.CharName} and {synergy.UserB.CharName}");
        foreach (var synergy in activeTriSynergies) Debug.Log($"Active Tri Synergy: {synergy.Ability.name} between {synergy.UserA.CharName}, {synergy.UserB.CharName}, and {synergy.UserC.CharName}");
    }

    public List<DualSynergyResult> GetAvailableDualSynergies(PlayerCharBattle userA, PlayerCharBattle userB)
    {
        if (userB == null) return activeDualSynergies.Where(s => s.UserA == userA.charData || s.UserB == userA.charData).ToList();
        else
        return activeDualSynergies.Where(s => (s.UserA == userA.charData && s.UserB == userB.charData) || (s.UserA == userB.charData && s.UserB == userA.charData)).ToList();
    }

    public List<TriSynergyResult> GetAvailableTriSynergies(PlayerCharBattle userA, PlayerCharBattle userB, PlayerCharBattle userC)
    {
        return activeTriSynergies.Where(s => {
            bool hasA = s.UserA == userA.charData || s.UserB == userA.charData || s.UserC == userA.charData;
            bool hasB = userB == null || (s.UserA == userB.charData || s.UserB == userB.charData || s.UserC == userB.charData);
            bool hasC = userC == null || (s.UserA == userC.charData || s.UserB == userC.charData || s.UserC == userC.charData);
            
            return hasA && hasB && hasC;
        }).ToList();
    }
}
