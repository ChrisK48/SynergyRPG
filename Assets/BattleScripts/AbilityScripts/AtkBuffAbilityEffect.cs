using UnityEngine;

[System.Serializable]
public class AtkBuffAbilityEffect : AtkAbilityEffect
{
    public Buff buff;
    public int duration;
    public int buffHitChance;

    public override void ApplyEffect(CharBattle[] users, CharBattle target, int calculatedPower)
    {
        int damage = calculateDamage(users, target, calculatedPower);
        target.TakeDamage(damage, atkType,ignoreDef);
        OnHit(users, target);
    }

    public void OnHit(CharBattle[] users, CharBattle target)
    {
        if (Random.value <= (buffHitChance / 100f))
        {
            target.ReceiveBuff(buff, duration);
            string userName = users.Length > 1 ? "The party" : users[0].CharName;
            Debug.Log($"{userName} gave {target.CharName} the {buff.buffName} buff for {duration} turns!");
        }
    }
}

