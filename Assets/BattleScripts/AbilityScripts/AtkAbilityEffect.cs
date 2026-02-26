using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
[System.Serializable]
public class AtkAbilityEffect : AbilityEffect
{
    public List<ElementType> elementTypes;
    public AtkType atkType;
    public bool ignoreDef;

    public override void ApplyEffect(CharBattle[] users, CharBattle target)
    {
        int damage = calculateDamage(users, target);
        target.TakeDamage(damage, atkType, ignoreDef);   
    }

    public float CheckIfCrit(CharBattle[] users, float damage)
    {

        // This is a placeholder for critical hit calculation. Currently a flat 5% chance to deal 50% more damage.
        if (Random.value > 0.95f - (GetUserStat(users) / 1000f))
        {
            damage *= 1.5f;
            if (users[0] is PlayerCharBattle) FlowManager.instance.GainFlow(10); // Temp 10 SP gain on crit, can be adjusted or removed later.
            Debug.Log("Critical hit dealt by " + users[0].charName + "!");
        }      

        return damage;
    }

    public int calculateDamage(CharBattle[] users, CharBattle target)
    {
        float damage = GetUserStat(users) * scalingMultiplier;

        if (atkType == AtkType.Physical)
        {
            damage = CheckIfCrit(users, damage);
        }

        // Temp weakness/resistance calculation. Currently doubles or halves damage based on a single matching element.
        if (target is NpcBattle npcTarget)
        {
            foreach (ElementType element in elementTypes)
            {
                if (npcTarget.elementalWeaknesses.Contains(element))
                {
                    damage *= 2;
                    Debug.Log("It's super effective!");
                    return (int)damage;
                }
                else if (npcTarget.elementalResistances.Contains(element))
                {
                    damage /= 2;
                    Debug.Log("It's not very effective...");
                    return (int)damage;
                }
            }
        }
        return (int)damage;
    }
}
