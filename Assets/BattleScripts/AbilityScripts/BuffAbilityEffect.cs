using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuffAbilityEffect : AbilityEffect
{
    public List<Buff> buffsToApply;
    public int buffDuration;
    public bool SharedInStance;

    public override void ApplyEffect(CharBattle[] users, ITurnEntity target, int calculatedPower)
    {
        foreach (Buff buffToApply in buffsToApply)
            target.ReceiveBuff(buffToApply, buffDuration);   
    }
}
