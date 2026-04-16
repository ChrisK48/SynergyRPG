using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipmentSlot
{
    [HideInInspector] public EquipSlot slotType;
    public Equippable currentItem;
    public List<Gem> equippedGems = new List<Gem>();
    
    public bool IsEmpty => currentItem == null;

    public void Equip(PlayerCharData charData, Equippable newItem)
    {
        currentItem = newItem;

        List<Gem> oldGems = new List<Gem>(equippedGems);
        equippedGems.Clear();
        foreach (GemSlot gemSlot in newItem.gemSlots) equippedGems.Add(null);
        for (int i = 0; i < Mathf.Min(oldGems.Count, equippedGems.Count); i++)
        {
            if (oldGems[i] != null)
            {
                EquipGem(oldGems[i], i, charData);
            }
        }
        charData.RefreshAllStats();
        charData.RefreshAbilities();
    }

    public void Unequip(PlayerCharData charData, Equippable itemToRemove)
    {
        currentItem = null;
        charData.RefreshAllStats();
    }

    public void EquipGem(Gem newGem, int gemSlotIndex, PlayerCharData charData)
    {
        if (currentItem != null && gemSlotIndex >= 0 && gemSlotIndex < currentItem.gemSlots.Count)
        {
            equippedGems[gemSlotIndex] = newGem;
            if (newGem.GemAbility != null) charData.abilities.Add(newGem.GemAbility);
            Debug.Log($"Equipped {newGem.ItemName} in slot {gemSlotIndex + 1} of {currentItem.ItemName}");
        }
    }
}