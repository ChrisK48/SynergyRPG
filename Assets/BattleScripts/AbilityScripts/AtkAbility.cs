using UnityEngine;

[CreateAssetMenu(fileName = "New Atk Ability", menuName = "Abilities/Attack Ability")]
public class AtkAbility : Ability
{
    public AtkType atkType;

    public override void ApplyEffect(CharBattle user, CharBattle target)
    {
        int damage = calculateDamage(user, target);
        target.TakeDamage(damage);
    }

    public int calculateDamage(CharBattle user, CharBattle target)
    {
        float damage = GetUserStat(user) * scalingMultiplier;

        // This is a placeholder for critical hit calculation. Currently a flat 5% chance to deal 50% more damage.
        if (Random.value > 0.95f - (user.Luck / 1000f))
        {
            damage *= 1.5f;
            if (user is PlayerCharBattle && atkType == AtkType.Physical) SynergyManager.instance.GainSP(10); // Temp 10 SP gain on crit, can be adjusted or removed later.
            Debug.Log("Critical hit dealt by " + user.charName + "!");
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
