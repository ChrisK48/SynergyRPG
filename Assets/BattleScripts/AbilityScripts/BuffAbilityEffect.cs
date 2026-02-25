using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuffAbilityEffect : AbilityEffect
{
    public List<Buff> buffsToApply;
    public int buffDuration;

    public override void ApplyEffect(CharBattle user, CharBattle target)
    {
        foreach (Buff buffToApply in buffsToApply)
            target.ReceiveBuff(buffToApply, buffDuration);   
    }
}
