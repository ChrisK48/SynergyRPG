using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Items/Item")]
public class Item : ScriptableObject, ITargetableAction
{
    public string itemName;
    public String Name => itemName;
    public String itemDescription;
    public List<ItemEffect> itemEffects;
    public TargetType targetType;
    public TargetType Targets => targetType;

    public void UseItem(CharBattle user, CharBattle target)
    {
        Debug.Log(user.charName + " uses " + Name + "!");
        foreach (ItemEffect effect in itemEffects)
        {
            effect.ApplyEffect(user, target);
        }
    }

    public void PerformAction(CharBattle user, List<CharBattle> targets)
    {
        foreach (CharBattle target in targets)
        {
            InventoryManager.instance.RemoveItem(this);
            UseItem(user, target);
        }
    }
}
