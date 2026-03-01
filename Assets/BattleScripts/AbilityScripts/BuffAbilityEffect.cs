using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuffAbilityEffect : AbilityEffect
{
    public List<Buff> buffsToApply;
    public int buffDuration;
    public bool AppliedtoUser;
    public bool SharedInStance;

    public override void ApplyEffect(CharBattle[] users, ITurnEntity target, int calculatedPower)
    {
        if (AppliedtoUser)
        {
            foreach (CharBattle user in users)
            {
                foreach (Buff buffToApply in buffsToApply)
                {
                    user.ReceiveBuff(buffToApply, buffDuration);
                }
            }
            return;
        }
        
        foreach (Buff buffToApply in buffsToApply)
            target.ReceiveBuff(buffToApply, buffDuration);   
    }
}
