using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class PlayerCharBattle : CharBattle
{
    [HideInInspector]
    public List<Ability> abilities;
    public GameObject uniqueUIPrefab;
    public event Action OnStatsChanged;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TriggerStatsUpdate();
    }

    public void InitializeStatsFromData(PlayerCharData data)
    {
        CharName = data.CharName;
        MaxHp = data.MaxHp;
        MaxMp = data.MaxMp;
        Atk = data.Atk;
        Mag = data.Mag;
        Def = data.Def;
        Mdef = data.Mdef;
        Spd = data.Spd;
        Acc = data.Acc;
        Eva = data.Eva;
        Luck = data.Luck;

        hp = data.currentHp;
        mp = data.currentMp;

        Debug.Log($"Initialized stats from data for {CharName}: MaxHp: {MaxHp}, MaxMp: {MaxMp}, Atk: {Atk}, Mag: {Mag}, Def: {Def}, Mdef: {Mdef}, Spd: {Spd}, Acc: {Acc}, Eva: {Eva}, Luck: {Luck}");

        abilities = new List<Ability>(data.abilities);
    }

    public void DeductResources(Ability ability)
    {
        bool hasUniqueCost = ability.UniqueResourceCost != null;
        ChangeMp(-ability.MpCost);
        hp -= ability.HpCost;
        if (hasUniqueCost) ability.UniqueResourceCost.PayCost(this);
        TriggerStatsUpdate();
    }

    public bool CanPerformAbility(Ability ability)
    {
        bool hasUniqueCost = ability.UniqueResourceCost != null;

        if (mp >= ability.MpCost && hp >= ability.HpCost && (!hasUniqueCost || ability.UniqueResourceCost.CanPayCost(this)))
        {
            return true;
        }
        else
        {
            Debug.Log($"CharName has {mp} MP and {hp} HP, but does not have {ability.MpCost} enough resources to perform {ability.Name}");
            return false;
        }
    }

    public override void TakeDamage(int amt, AtkType atkType, List<DamageType> elementTypes, int shieldsToRemove = 0, bool ignoreDef = false, System.Action<int> onDamageDealt = null)
    {
        base.TakeDamage(amt, atkType, elementTypes, 0, ignoreDef, onDamageDealt);
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

    protected override void Die()
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
