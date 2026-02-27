using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : ScriptableObject, ITargetableAction
{
    public string itemName;
    public String Name => itemName;
    public String itemDescription;

    public TargetType targetType;
    public TargetType Targets => targetType;

    [SerializeReference]
    public List<ItemEffect> itemEffects = new List<ItemEffect>();

    public void UseItem(CharBattle user, CharBattle target)
    {
        Debug.Log(user.CharName + " uses " + Name + "!");
        foreach (ItemEffect effect in itemEffects)
        {
            effect.Apply(user, target);
        }
    }

    public void PerformAction(CharBattle[] user, List<CharBattle> targets)
    {
        InventoryManager.instance.RemoveItem(this);
        foreach (CharBattle target in targets)
        {
            UseItem(user[0], target);
        }
    }
}
