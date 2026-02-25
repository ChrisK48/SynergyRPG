using UnityEngine;

[System.Serializable]
public class AtkBuffAbilityEffect : AtkAbilityEffect
{
    public Buff buff;
    public int duration;
    public int buffHitChance;

    public override void ApplyEffect(CharBattle user, CharBattle target)
    {
        int damage = calculateDamage(user, target);
        target.TakeDamage(damage, atkType,ignoreDef);
        OnHit(user, target);
    }

    public void OnHit(CharBattle user, CharBattle target)
    {
        if (Random.value <= (buffHitChance / 100f))
        {
            target.ReceiveBuff(buff, duration);
            Debug.Log($"{target.charName} received {buff.buffName} buff for {duration} turns!");
        }
    }
}

