using UnityEngine;

[CreateAssetMenu(fileName = "New Atk Ability", menuName = "Abilities/Attack Ability")]
public class AtkAbility : Ability
{
    public AtkType atkType;

    protected override void ApplyEffect(CharBattle user, CharBattle target)
    {
        int damage = calculateDamage(user, target);
        target.TakeDamage(damage);
    }

    public int calculateDamage(CharBattle user, CharBattle target)
    {
        int damage = 0;

        // Temp weakness/resistance calculation. Currently doubles or halves damage based on a single matching element.
        if (target is NpcBattle npcTarget)
        {
            foreach (ElementType element in elementTypes)
            {
                if (npcTarget.elementalWeaknesses.Contains(element))
                {
                    damage = GetUserStat(user) * scalingMultiplier * 2;
                    Debug.Log("It's super effective!");
                    return damage;
                }
                else if (npcTarget.elementalResistances.Contains(element))
                {
                    damage = GetUserStat(user) * scalingMultiplier / 2;
                    Debug.Log("It's not very effective...");
                    return damage;
                }
            }
        }

        damage = GetUserStat(user) * scalingMultiplier;
        return damage;
    }
}
