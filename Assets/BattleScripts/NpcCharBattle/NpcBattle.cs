using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public abstract class NpcBattle : CharBattle
{
    public List<AbilityWeight> AbilityWeights = new List<AbilityWeight>();
    public List<HpGate> HpGates = new List<HpGate>();
    public List<DamageType> DamageResistances;
    public List<ShieldTag> DamageWeaknesses;
    public int shieldsToRegain;
    public int xpValue;
    private bool shieldBroken = false;
    private int DefNotBroken;
    private int MdefNotBroken;
    private bool isDead = false;

    public void Start()
    {
        DefNotBroken = Def;
        MdefNotBroken = Mdef;
        ResetShields();
    } 

    public virtual void PerformAITurn()
    {
        Ability selectedAbility = NpcAbilitySelection();
        List <ITurnEntity> targets = NpcTargeting(selectedAbility);
        PerformAbility(selectedAbility, targets);
        EndTurn();
    }

    public virtual Ability NpcAbilitySelection()
    {
        Ability chosenAbility = AbilityWeights[GetWeightedRandomIndex(AbilityWeights)].Ability;
        return chosenAbility;
    }

    protected void PerformAbility(Ability ability, List<ITurnEntity> targets)
    {
        ability.PerformAction(new CharBattle[] {this}, targets);
    }

    protected override void Die()
    {
        base.Die();
        BattleManager.instance.npcChars.Remove(this);

        if (!isDead) BattleManager.instance.AddEarnedXp(xpValue);
        isDead = true;
        // By default, NPCs will be removed from the battle when they die.
        Destroy(this.gameObject);
    }

    public override void TakeDamage(int amt, AtkType atkType, List<DamageType> damageTypes, int shieldsToRemove = 0, bool ignoreDef = false, Action<int> onDamageDealt = null)
    {
        float damage = HandleWeaknesses(amt, damageTypes);
        damage = HandleResistances(damage, damageTypes);
        amt = (int)damage;
        base.TakeDamage(amt, atkType, damageTypes, shieldsToRemove, ignoreDef, onDamageDealt);
        DecrementShieldTags(damageTypes, shieldsToRemove);
    }

    protected List<ITurnEntity> NpcTargeting(Ability ability)
    {
        switch (ability.TargetType)
        {
            case TargetType.SingleEnemy:
                int randomAllyIndex = UnityEngine.Random.Range(0, BattleManager.instance.playerEntities.Count);
                return new List<ITurnEntity> { BattleManager.instance.playerEntities[randomAllyIndex] };
            case TargetType.AllEnemies:
                return BattleManager.instance.playerEntities.Where(p => p is PlayerCharBattle player && player.GetIfAlive() || p is SynergyStance).ToList();
            case TargetType.Self:
                return new List<ITurnEntity> { this };
            case TargetType.SingleAlly:
                int randomEnemyIndex = UnityEngine.Random.Range(0, BattleManager.instance.npcEntities.Count);
                return new List<ITurnEntity> { BattleManager.instance.npcEntities[randomEnemyIndex] };
            case TargetType.AllAllies:
                return BattleManager.instance.npcEntities;
            case TargetType.AnyChar:
                List<ITurnEntity> allChars = new List<ITurnEntity>();
                allChars.AddRange(BattleManager.instance.playerEntities);
                allChars.AddRange(BattleManager.instance.npcEntities);
                int randomCharIndex = UnityEngine.Random.Range(0, allChars.Count);
                return new List<ITurnEntity> { allChars[randomCharIndex] };
            case TargetType.AllChars:
                List<ITurnEntity> allCharacters = new List<ITurnEntity>();
                allCharacters.AddRange(BattleManager.instance.playerEntities);
                allCharacters.AddRange(BattleManager.instance.npcEntities);
                return allCharacters;
            case TargetType.RandomAllies:
                return BattleManager.instance.npcEntities;
            case TargetType.RandomEnemies:
                return BattleManager.instance.playerEntities.Where(p => p is PlayerCharBattle player && player.GetIfAlive() || p is SynergyStance).ToList();
            default:
                return new List<ITurnEntity>();
        }
    }

    public int GetWeightedRandomIndex(List<AbilityWeight> weights)
    {
        int totalWeight = 0;
        foreach (var w in weights) totalWeight += w.Weight;

        int roll = UnityEngine.Random.Range(0, totalWeight);
        int cursor = 0;

        for (int i = 0; i < weights.Count; i++)
        {
            cursor += weights[i].Weight;
            if (roll < cursor) return i;
        }
        return 0;
    }

    public bool CheckHpGates()
    {
        foreach (var gate in HpGates)
        {
            if (!gate.HasTriggered && hp <= MaxHp * gate.Threshold)
            {
                gate.HasTriggered = true;
                return true;
            }
        }
        return false;
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
