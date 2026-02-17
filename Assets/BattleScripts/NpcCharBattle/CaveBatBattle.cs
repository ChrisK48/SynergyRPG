using UnityEngine;
using System.Collections.Generic;

public class CaveBatBattle : NpcBattle
{
    public override void PerformAITurn()
    {
        List<CharBattle> targets = new List<CharBattle>();
        targets.Add(NpcTargetingLogic(abilities[0]));
        PerformAbility(abilities[0], targets);
        BattleManager.instance.NextTurn();
    }

    public override void PerformAbility(Ability ability, List<CharBattle> targets)
    {
        foreach (CharBattle target in targets)
        {
            Debug.Log(charName + " uses " + ability.abilityName + " on " + target.charName);
            ability.ExecuteAbility(this, target);
        }
    }

    public override CharBattle NpcTargetingLogic(Ability ability)
    {
        int randomIndex = Random.Range(0, BattleManager.instance.alivePlayerChars.Count);
        CharBattle target = BattleManager.instance.alivePlayerChars[randomIndex];
        return target;
    }
}
