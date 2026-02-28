using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class NpcBattle : CharBattle
{
    public int exp;
    public List<ElementType> elementalWeaknesses;
    public List<ElementType> elementalResistances;

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

    public override void Die()
    {
        base.Die();
        BattleManager.instance.npcChars.Remove(this);

        // By default, NPCs will be removed from the battle when they die.
        Destroy(this.gameObject);
    }
}
