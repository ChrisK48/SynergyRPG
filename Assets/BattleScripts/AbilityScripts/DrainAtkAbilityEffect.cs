using UnityEngine;

[System.Serializable]
public class DrainAtkAbilityEffect : AtkAbilityEffect
{
    public float drainPercentage;

    public override void ApplyEffect(CharBattle[] users, ITurnEntity target, int calculatedPower)
    {
        int power = calculateDamage(users, target, calculatedPower);        
        target.TakeDamage(power, atkType, elementTypes, ShieldsToRemove, ignoreDef, (finalAmount) => {
            int heal = Mathf.RoundToInt((finalAmount * (drainPercentage / 100f)) / users.Length);
            foreach (CharBattle u in users)
            {
                u.Heal(heal);
            }
            string userName = users.Length > 1 ? "The party" : users[0].CharName;
            Debug.Log($"{userName} drained {heal} HP!");
        });
    }
}
