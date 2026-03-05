using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Mathematics;

public abstract class NpcBattle : CharBattle
{
    public int exp;
    public List<DamageType> DamageResistances;
    public List<ShieldTag> DamageWeaknesses;
    public int xpValue;
    public int shieldsToRegain;
    private bool shieldBroken = false;
    private int DefNotBroken;
    private int MdefNotBroken;

    public void Start()
    {
        DefNotBroken = Def;
        MdefNotBroken = Mdef;
        ResetShields();
    } 

    public virtual void PerformAITurn()
    {
        List<ITurnEntity> targets = new List<ITurnEntity>();
        Ability selectedAbility = NpcAbilitySelectionLogic();
        targets.Add(NpcTargetingLogic(selectedAbility));
        PerformAbility(selectedAbility, targets);
        BattleManager.instance.NextTurn();   
    }

    public abstract ITurnEntity NpcTargetingLogic(Ability ability);

    public abstract Ability NpcAbilitySelectionLogic();



    public void PerformAbility(Ability ability, List<ITurnEntity> targets)
    {
        foreach (ITurnEntity target in targets)
        {
            ability.PerformAction(new CharBattle[] {this}, new List<ITurnEntity> {target});
        }
    }

    protected List<ITurnEntity> GetPotentialTargets()
    {
        return BattleManager.instance.playerEntities.Where(pc => (pc is PlayerCharBattle player && player.GetIfAlive()) || (pc is SynergyStance synergyStance)).ToList();
    }

    protected override void Die()
    {
        base.Die();
        BattleManager.instance.npcChars.Remove(this);

        BattleManager.instance.AddEarnedXp(xpValue);
        // By default, NPCs will be removed from the battle when they die.
        Destroy(this.gameObject);
    }

    public override void TakeDamage(int amt, AtkType atkType, List<DamageType> damageTypes, int shieldsToRemove = 0, bool ignoreDef = false, Action<int> onDamageDealt = null)
    {
        Debug.Log("Damage Dealt prior to resistances/weaknesses: " + amt);
        float damage = HandleWeaknesses(amt, damageTypes);
        Debug.Log("Damage after weaknesses: " + damage);
        damage = HandleResistances(damage, damageTypes);
        Debug.Log("Damage after resistances: " + damage);
        amt = (int)damage;
        base.TakeDamage(amt, atkType, damageTypes, shieldsToRemove, ignoreDef, onDamageDealt);
        DecrementShieldTags(damageTypes, shieldsToRemove);
    }

    private float HandleWeaknesses(float amt, List<DamageType> damageTypes)
    {
        float damage = (float)amt;
        foreach (var weakness in DamageWeaknesses)
        {
            if (damageTypes.Contains(weakness.element))
            {
                damage *= 1.5f;
                return damage;
            }
        }
        return damage;
    }

    private float HandleResistances(float amt, List<DamageType> damageTypes)
    {
        float damage = (float)amt;
        foreach (var resistance in DamageResistances)
        {
            if (damageTypes.Contains(resistance))
            {
                damage *= 0.5f;
                return damage;
            }
        }
        return damage;
    }

    private void DecrementShieldTags(List<DamageType> elementTypes, int shieldsToRemove)
    {
        if (elementTypes == null) return;
        foreach (var shieldTag in DamageWeaknesses)
        {
            if (elementTypes.Contains(shieldTag.element))
            {
                shieldTag.shieldAmount = Mathf.Max(0, shieldTag.shieldAmount - shieldsToRemove);
                Debug.Log(CharName + "'s " + shieldTag.element + " shield is reduced to " + shieldTag.shieldAmount);
                if (shieldTag.shieldAmount <= 0)
                {
                    BreakShieldTag(shieldTag);
                }
            }
        }
    }

    private void BreakShieldTag(ShieldTag shieldTag)
    {
        Debug.Log(CharName + "'s " + shieldTag.element + " shield is broken!");
        bool isStillShielded = DamageWeaknesses.Any(tag => tag.shieldAmount > 0);
        if (!isStillShielded)
        {
            TriggerShieldBreak();
        }
    }

    protected virtual void TriggerShieldBreak()
    {
        Debug.Log(CharName + " has lost all shields and is now vulnerable!");
        Def = (int)(DefNotBroken * 0.8f); // currently reduces defense by 20% when all shields are broken, can adjust as needed
        Mdef = (int)(Mdef * 0.8f); // also reduces magic defense by 20%
        shieldBroken = true;
        BattleManager.instance.GetTurnManager().RemoveFromTurnOrder(this);
    }

    public bool IsShieldBroken() => shieldBroken;

    public void ResetShields()
    {
        foreach (var shield in DamageWeaknesses)
        {
            shield.ResetShieldAmount();
        }
        shieldBroken = false;
        Def = DefNotBroken;
        Mdef = MdefNotBroken;
        Debug.Log(CharName + "'s shields have been reset.");
    }
}
