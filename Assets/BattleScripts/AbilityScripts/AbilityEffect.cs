using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public abstract class AbilityEffect
{
    public ScalingStat scalingStat; 
    public int scalingMultiplier;
    public bool effectIsMissable;
    public int effectHitChance;
    public int numHits;

    public void ExecuteEffect(CharBattle user, CharBattle target)
    {
        for (int i = 0; i < numHits; i++)
         {
             if (!CheckIfHit(user, target)) 
             {
                continue;
             }
             ApplyEffect(user, target);
         }
    }
    public abstract void ApplyEffect(CharBattle user, CharBattle target);

    protected int GetUserStat(CharBattle user)
    {
        switch (scalingStat)
        {
            case ScalingStat.Atk:
                return user.Atk;
            case ScalingStat.Mag:
                return user.Mag;
            case ScalingStat.Def:
                return user.Def;
            case ScalingStat.Mdef:
                return user.Mdef;
            case ScalingStat.Spd:
                return user.Spd;
            case ScalingStat.Acc:
                return user.Acc;
            case ScalingStat.Eva:
                return user.Eva;
            case ScalingStat.Luck:
                return user.Luck;
            default:
                return 0;
        }
    }

    protected bool CheckIfHit(CharBattle user, CharBattle target)
    {
        if (effectIsMissable)
        {
            // temp hit calculation. Need to account for target's evasion later.
            if (Random.value <= (effectHitChance / 100f))
            {
                return true;
            } else
            {
                Debug.Log($"{user.charName}'s ability missed {target.charName}!");
                return false;
            }
        }
        return true;
    }
}
