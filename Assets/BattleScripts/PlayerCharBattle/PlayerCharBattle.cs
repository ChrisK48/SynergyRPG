using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class PlayerCharBattle : CharBattle
{
    public GameObject uniqueUIPrefab;
    public event Action OnStatsChanged;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TriggerStatsUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool CanPerformAbility(Ability ability, List<CharBattle> targets)
    {
        bool hasUniqueCost = ability.UniqueResourceCost != null;

        if (mp >= ability.MpCost && hp >= ability.HpCost && (!hasUniqueCost || ability.UniqueResourceCost.CanPayCost(this)))
        {
            mp -= ability.MpCost;
            hp -= ability.HpCost;
            if (hasUniqueCost) ability.UniqueResourceCost.PayCost(this);
            TriggerStatsUpdate();
            return true;
        }
        else
        {
            Debug.Log(CharName + " does not have enough resources to perform " + ability.Name);
            return false;
        }
    }

    public override void TakeDamage(int amt, AtkType atkType, bool ignoreDef = false, System.Action<int> onDamageDealt = null)
    {
        base.TakeDamage(amt, atkType, ignoreDef, onDamageDealt);
        TriggerStatsUpdate();
    }

    public override void Heal(int amt)
    {
        base.Heal(amt);
        TriggerStatsUpdate();
    }

    public void ChangeMp(int amt)
    {
        Debug.Log(CharName + (amt >= 0 ? " restores " : " loses ") + Mathf.Abs(amt) + " MP.");
        mp = Mathf.Clamp(mp + amt, 0, MaxMp);
        TriggerStatsUpdate();
    }

    public void DeductPreppedAbilityResourceCosts()
    {
        hp -= preppedAbility.HpCost;
        mp -= preppedAbility.MpCost;
        if (preppedAbility.UniqueResourceCost != null) preppedAbility.UniqueResourceCost.PayCost(this);
        TriggerStatsUpdate();
    }

    protected void TriggerStatsUpdate() => OnStatsChanged?.Invoke();

    public override void Die()
    {
        base.Die();
    }

    public void Revive()
    {
        if (hp > 0)
        {
            Debug.Log(CharName + " is not dead and cannot be revived.");
            return;
        }
        hp = 1;
        isAlive = true;
        Debug.Log(CharName + " has been revived!");
        TriggerStatsUpdate();
    }
}
