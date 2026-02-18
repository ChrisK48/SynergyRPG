using UnityEngine;


public class DrainAtkAbility : AtkAbility
{
    public float drainPercentage;

    public override void ExecuteAbility(CharBattle user, CharBattle target)
    {
        int power = calculateDamage(user);        
        target.TakeDamage(power, (finalAmount) => {
            int heal = Mathf.RoundToInt(finalAmount * (drainPercentage / 100f));
            user.Heal(heal);
            Debug.Log($"{user.charName} drained {heal} HP!");
        });
    }
}
