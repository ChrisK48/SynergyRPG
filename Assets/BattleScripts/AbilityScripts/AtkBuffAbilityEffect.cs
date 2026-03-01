using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AtkBuffAbilityEffect : AtkAbilityEffect
{
    public List<Buff> buffsToApply;
    public int duration;
    public bool buffIsMissable;
    public int buffHitChance;
    public bool AppliedToUser;

    public override void ApplyEffect(CharBattle[] users, ITurnEntity target, int calculatedPower)
    {
        int damage = calculateDamage(users, target, calculatedPower);
        target.TakeDamage(damage, atkType, elementTypes, ShieldsToRemove, ignoreDef);
        OnHit(users, target);
    }

    public void OnHit(CharBattle[] users, ITurnEntity target)  
    {
        if (!buffIsMissable || Random.value <= (buffHitChance / 100f))
        {
            if (AppliedToUser)
            {
                foreach (CharBattle user in users)
                {
                    foreach (Buff buffToApply in buffsToApply)
                    {
                        user.ReceiveBuff(buffToApply, duration);
                    }
                }
                return;
            }
            foreach (Buff buff in buffsToApply)
            {
                target.ReceiveBuff(buff, duration);
            }
        }
    }
}

