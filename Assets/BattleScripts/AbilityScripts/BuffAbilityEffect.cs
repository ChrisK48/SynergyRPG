using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuffAbilityEffect : AbilityEffect
{
    public List<Buff> buffsToApply;
    public int buffDuration;

    public override void ApplyEffect(CharBattle[] users, CharBattle target, int calculatedPower)
    {
        foreach (Buff buffToApply in buffsToApply)
            target.ReceiveBuff(buffToApply, buffDuration);   
    }
}
