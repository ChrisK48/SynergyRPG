using UnityEngine;

[System.Serializable]
public class AtkBuffAbilityEffect : AtkAbilityEffect
{
    public Buff buff;
    public int duration;
    public int buffHitChance;

    public override void ApplyEffect(CharBattle[] users, CharBattle target)
    {
        int damage = calculateDamage(users, target);
        target.TakeDamage(damage, atkType,ignoreDef);
        OnHit(users, target);
    }

    public void OnHit(CharBattle[] users, CharBattle target)
    {
        if (Random.value <= (buffHitChance / 100f))
        {
            target.ReceiveBuff(buff, duration);
            string userName = users.Length > 1 ? "The party" : users[0].charName;
            Debug.Log($"{userName} gave {target.charName} the {buff.buffName} buff for {duration} turns!");
        }
    }
}

