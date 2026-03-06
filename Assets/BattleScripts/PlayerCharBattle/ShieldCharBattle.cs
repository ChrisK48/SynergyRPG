using System;
using System.Collections.Generic;
using UnityEngine;

public class ShieldCharBattle : PlayerCharBattle
{
    public int maxShieldPoints = 10;
    public int shieldPoints = 0;

    public void GainShield(int amt)
    {
        shieldPoints = Mathf.Clamp(shieldPoints + amt, 0, maxShieldPoints);
        TriggerStatsUpdate();
    }

    public void LoseShield(int amt)
    {
        shieldPoints = Mathf.Clamp(shieldPoints - amt, 0, maxShieldPoints);
        TriggerStatsUpdate();
    }

    public bool CanPayShieldCost(int cost)
    {
        return shieldPoints >= cost;
    }

    public override void TakeDamage(int amt, AtkType atkType, List<DamageType> elementTypes, int shieldsToRemove = 0, bool ignoreDef = false, Action<int> onDamageDealt = null)
    {
        base.TakeDamage(amt, atkType, elementTypes, 0, ignoreDef, onDamageDealt);
        if (isDefending) GainShield(1); // temporary: gain 5 shield points when defending and hit, can be changed to be more dynamic later
        TriggerStatsUpdate();
    }
}
