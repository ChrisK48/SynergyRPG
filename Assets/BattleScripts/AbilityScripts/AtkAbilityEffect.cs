using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
[System.Serializable]
public class AtkAbilityEffect : AbilityEffect
{
    public List<DamageType> elementTypes;
    public AtkType atkType;
    public bool ignoreDef;

    public override void ApplyEffect(CharBattle[] users, ITurnEntity target, int calculatedPower)
    {
        int damage = calculateDamage(users, target, calculatedPower);
        target.TakeDamage(damage, atkType, elementTypes, ShieldsToRemove, ignoreDef);   
    }

    public float CheckIfCrit(CharBattle[] users, float damage)
    {
        float totalLuck = 0;
        foreach (CharBattle user in users)
        {
            totalLuck += (float)user.Luck;
        }

        totalLuck /= users.Length; // Average luck of all users for the attack

        // This is a placeholder for critical hit calculation. Currently a flat 5% chance to deal 50% more damage.
        if (Random.value > 0.95f - (totalLuck / 1000f))
        {
            damage *= 1.5f;
            if (users[0] is PlayerCharBattle) FlowManager.instance.GainFlow(10); // Temp 10 SP gain on crit, can be adjusted or removed later.
            Debug.Log("Critical hit dealt by " + users[0].CharName + "!");
        }      

        return damage;
    }

    public int calculateDamage(CharBattle[] users, ITurnEntity target, int calculatedPower)
    {
        float damage = calculatedPower;

        if (atkType == AtkType.Physical)
        {
            damage = CheckIfCrit(users, damage);
        }

        // Temp weakness/resistance calculation. Currently doubles or halves damage based on a single matching element.
        if (target is NpcBattle npcTarget)
        {
            foreach (DamageType element in elementTypes)
            {
                if (npcTarget.DamageWeaknesses.Exists(tag => tag.element == element))
                {
                    damage *= 2;
                    Debug.Log("It's super effective!");
                    return (int)damage;
                }
                else if (npcTarget.DamageResistances.Contains(element))
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
