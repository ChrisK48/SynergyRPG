using UnityEngine;

[System.Serializable]
public class DrainAtkAbilityEffect : AtkAbilityEffect
{
    public float drainPercentage;

    public override void ApplyEffect(CharBattle[] users, CharBattle target)
    {
        int power = calculateDamage(users, target);        
        target.TakeDamage(power, atkType, ignoreDef, (finalAmount) => {
            int heal = Mathf.RoundToInt((finalAmount * (drainPercentage / 100f)) / users.Length);
            foreach (CharBattle u in users)
            {
                u.Heal(heal);
            }
            string userName = users.Length > 1 ? "The party" : users[0].charName;
            Debug.Log($"{userName} drained {heal} HP!");
        });
    }
}
