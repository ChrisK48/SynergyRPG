using UnityEngine;

[System.Serializable]
public class DrainAtkAbilityEffect : AtkAbilityEffect
{
    public float drainPercentage;

    public override void ApplyEffect(CharBattle user, CharBattle target)
    {
        int power = calculateDamage(user, target);        
        target.TakeDamage(power, atkType, ignoreDef, (finalAmount) => {
            int heal = Mathf.RoundToInt(finalAmount * (drainPercentage / 100f));
            user.Heal(heal);
            Debug.Log($"{user.charName} drained {heal} HP!");
        });
    }
}
