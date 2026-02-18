using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class NpcBattle : CharBattle
{
    public int exp;

    public virtual void PerformAITurn()
    {
        List<CharBattle> targets = new List<CharBattle>();
        Ability selectedAbility = NpcAbilitySelectionLogic();
        targets.Add(NpcTargetingLogic(selectedAbility));
        PerformAbility(selectedAbility, targets);
        BattleManager.instance.NextTurn();   
    }

    public abstract CharBattle NpcTargetingLogic(Ability ability);

    public abstract Ability NpcAbilitySelectionLogic();

    public override void PerformAbility(Ability ability, List<CharBattle> targets)
    {
        foreach (CharBattle target in targets)
        {
            Debug.Log(charName + " uses " + ability.abilityName + " on " + target.charName);
            ability.ExecuteAbility(this, target);
        }
    }

    public override void Die()
    {
        base.Die();
        BattleManager.instance.npcChars.Remove(this);

        // By default, NPCs will be removed from the battle when they die.
        Destroy(this.gameObject);
    }
}
