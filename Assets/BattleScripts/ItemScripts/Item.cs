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
        foreach (ItemEffect effect in itemEffects)
        {
            effect.Apply(user, target);
        }
    }

    public void PerformAction(CharBattle[] user, List<ITurnEntity> targets, System.Action onComplete = null)
    {
        InventoryManager.instance.RemoveItem(this);
        foreach (ITurnEntity target in targets)
        {
            if (target is CharBattle charTarget)
            {
                UseItem(user[0], charTarget);
            }

            if (target is SynergyStance synergyStance)
            {
                foreach (var stanceUser in synergyStance.users)
                {
                    UseItem(user[0], stanceUser);
                }
            }
        }
        onComplete?.Invoke();
    }
}
