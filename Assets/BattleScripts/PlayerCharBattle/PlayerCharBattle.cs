using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerCharBattle : CharBattle
{
    public GameObject uniqueUIPrefab;
    public event Action OnStatsChanged;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void PerformAction(ITargetableAction action, List<CharBattle> targets)
    {
        if (action is Ability ability) 
        {
            PerformAbility(ability, targets);
        }
        else
        {
            // logic for items or other actions
        }
    }

    public void PerformAbility(Ability ability, List<CharBattle> targets)
    {
        if (Mp >= ability.mpCost && Hp >= ability.hpCost)
        {
            Mp -= ability.mpCost;
            Hp -= ability.hpCost;
            TriggerStatsUpdate();
            foreach (CharBattle target in targets)
            {
                ability.ExecuteAbility(this, target);
            }
        }
        else
        {
            Debug.Log(charName + " does not have enough resources to perform " + ability.Name);
        }
    }

    public override void TakeDamage(int amt, System.Action<int> onDamageDealt = null)
    {
        base.TakeDamage(amt, onDamageDealt);
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

    protected void TriggerStatsUpdate() => OnStatsChanged?.Invoke();

    public override void Die()
    {
        base.Die();
    }
}
