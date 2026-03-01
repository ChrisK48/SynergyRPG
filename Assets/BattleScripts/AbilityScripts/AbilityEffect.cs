using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public abstract class AbilityEffect
{
    public bool EffectIsMissable;
    public int EffectHitChance;
    public int NumHits;
    public int ShieldsToRemove;

    public void ExecuteEffect(CharBattle[] users, ITurnEntity target, int calculatedPower)
    {
        for (int i = 0; i < NumHits; i++)
        {
            if (EffectIsMissable)
            {
                if (!CheckIfHit(users, target)) 
                {
                    continue;
                }
            }
            ApplyEffect(users, target, calculatedPower);
        }
    }
    public abstract void ApplyEffect(CharBattle[] users, ITurnEntity target, int calculatedPower);

    protected bool CheckIfHit(CharBattle[] users, ITurnEntity target)
    {

        // temp hit calculation. Need to account for target's evasion later.
        if (Random.value <= (EffectHitChance / 100f))
        {
            return true;
        }

        string userName = users.Length > 1 ? "The party" : users[0].CharName;
        Debug.Log($"{userName} missed {target.EntityName}!");
        return false;
    }
}
