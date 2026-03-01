using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class NpcBattle : CharBattle
{
    public int exp;
    public List<DamageType> DamageResistances;
    public List<ShieldTag> DamageWeaknesses;
    private bool shieldBroken = false;

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

        // By default, NPCs will be removed from the battle when they die.
        Destroy(this.gameObject);
    }

    public override void TakeDamage(int amt, AtkType atkType, List<DamageType> elementTypes, int shieldsToRemove = 0, bool ignoreDef = false, Action<int> onDamageDealt = null)
    {
        base.TakeDamage(amt, atkType, elementTypes, shieldsToRemove, ignoreDef, onDamageDealt);
        DecrementShieldTags(elementTypes, shieldsToRemove);
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
            Debug.Log(CharName + " has lost all shields and is now vulnerable to elemental weaknesses!");
            TriggerShieldBreak();
        }
    }

    protected virtual void TriggerShieldBreak()
    {
        shieldBroken = true;
        BattleManager.instance.GetTurnManager().OnNpcShieldBroken(this);
    }

    public bool IsShieldBroken() => shieldBroken;
}
