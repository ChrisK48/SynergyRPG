using System;
using System.Collections.Generic;
using UnityEngine;

public class ShieldCharBattle : PlayerCharBattle
{
    public int maxShieldPoints = 10;
    public int shieldPoints = 0;
    private bool ShieldStatus = false;

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

    public int GetShieldPoints() => shieldPoints;

    public bool CanPayShieldCost(int cost)
    {
        return shieldPoints >= cost;
    }

    public override void TakeDamage(int amt, AtkType atkType, List<DamageType> elementTypes, int shieldsToRemove = 0, bool ignoreDef = false, Action<int> onDamageDealt = null)
    {
        base.TakeDamage(amt, atkType, elementTypes, 0, ignoreDef, onDamageDealt);
        if (isDefending) GainShield(2);
        TriggerStatsUpdate();
    }

    public void ActivateGuardianShield()
    {
        ShieldStatus = true;
    }

    public void HandleGuardianShieldHit()
    {
        LoseShield(2);
        if (shieldPoints == 0) EndGuardianShield();
    }

    private void EndGuardianShield()
    {
        ShieldStatus = false;
    }

    public bool getShieldStatus => ShieldStatus;
}
