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

    public override void PerformAbility(Ability ability, List<CharBattle> targets)
    {
        if (Mp >= ability.mpCost && Hp >= ability.hpCost)
        {
            Mp -= ability.mpCost;
            Hp -= ability.hpCost;
            OnStatsChanged?.Invoke();
            foreach (CharBattle target in targets)
            {
                ability.ExecuteAbility(this, target);
            }
        }
        else
        {
            Debug.Log(charName + " does not have enough resources to perform " + ability.abilityName);
        }
    }

    public override void TakeDamage(int amt, System.Action<int> onDamageDealt = null)
    {
        base.TakeDamage(amt, onDamageDealt);
        OnStatsChanged?.Invoke();
    }

    public override void Heal(int amt)
    {
        base.Heal(amt);
        OnStatsChanged?.Invoke();
    }

    public override void Die()
    {
        base.Die();
        BattleManager.instance.alivePlayerChars.Remove(this);
    }
}
