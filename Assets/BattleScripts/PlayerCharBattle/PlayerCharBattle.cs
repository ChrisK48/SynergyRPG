using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class PlayerCharBattle : CharBattle
{
    public GameObject uniqueUIPrefab;
    public event Action OnStatsChanged;
    private Ability storedSynergy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool CanPerformAbility(Ability ability, List<CharBattle> targets)
    {
        bool hasUniqueCost = ability.uniqueResourceCost != null;

        if (Mp >= ability.mpCost && Hp >= ability.hpCost && (!hasUniqueCost || ability.uniqueResourceCost.CanPayCost(this)))
        {
            Mp -= ability.mpCost;
            Hp -= ability.hpCost;
            if (hasUniqueCost) ability.uniqueResourceCost.PayCost(this);
            TriggerStatsUpdate();
            return true;
        }
        else
        {
            Debug.Log(charName + " does not have enough resources to perform " + ability.Name);
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
        Debug.Log(charName + (amt >= 0 ? " restores " : " loses ") + Mathf.Abs(amt) + " MP.");
        Mp = Mathf.Clamp(Mp + amt, 0, maxMp);
        TriggerStatsUpdate();
    }

    public void StoreSynergy(Ability synergy)
    {
        storedSynergy = synergy;
    }

    public void DeductSynergyResourceCosts()
    {
        Hp -= storedSynergy.hpCost;
        Mp -= storedSynergy.mpCost;
        if (storedSynergy.uniqueResourceCost != null) storedSynergy.uniqueResourceCost.PayCost(this);
        TriggerStatsUpdate();
    }

    protected void TriggerStatsUpdate() => OnStatsChanged?.Invoke();

    public override void Die()
    {
        base.Die();
    }
}
