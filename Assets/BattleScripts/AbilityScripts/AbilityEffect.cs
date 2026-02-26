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

    public void ExecuteEffect(CharBattle[] users, CharBattle target)
    {
        for (int i = 0; i < numHits; i++)
        {
            if (effectIsMissable)
            {
                if (!CheckIfHit(users, target)) 
                {
                    continue;
                }
            }
            ApplyEffect(users, target);
        }
    }
    public abstract void ApplyEffect(CharBattle[] users, CharBattle target);

    protected int GetUserStat(CharBattle[] users)
    {
        int stat = 0;
        switch (scalingStat)
        {
            case ScalingStat.Atk:
                foreach (CharBattle user in users)
                {
                    stat += user.Atk;
                }
                return stat;
            case ScalingStat.Mag:
                foreach (CharBattle user in users)
                {
                    stat += user.Mag;
                }
                return stat;
            case ScalingStat.Def:
                foreach (CharBattle user in users)
                {
                    stat += user.Def;
                }
                return stat;
            case ScalingStat.Mdef:
                foreach (CharBattle user in users)
                {
                    stat += user.Mdef;
                }
                return stat;
            case ScalingStat.Spd:
                foreach (CharBattle user in users)
                {
                    stat += user.Spd;
                }
                return stat;
            case ScalingStat.Acc:
                foreach (CharBattle user in users)
                {
                    stat += user.Acc;
                }
                return stat;
            case ScalingStat.Eva:
                foreach (CharBattle user in users)
                {
                    stat += user.Eva;
                }
                return stat;
            case ScalingStat.Luck:
                foreach (CharBattle user in users)      
                {
                    stat += user.Luck;
                }
                return stat;
            default:
                return 0;
        }   
    }

    protected bool CheckIfHit(CharBattle[] users, CharBattle target)
    {

        // temp hit calculation. Need to account for target's evasion later.
        if (Random.value <= (effectHitChance / 100f))
        {
            return true;
        }

        string userName = users.Length > 1 ? "The party" : users[0].charName;
        Debug.Log($"{userName} missed {target.charName}!");
        return false;
    }
}
