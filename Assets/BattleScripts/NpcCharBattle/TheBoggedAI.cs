using UnityEngine;
using System.Collections.Generic;

public class TheBoggedAI : NpcBattle
{
    public override Ability NpcAbilitySelection()
    {
        Ability chosenAbility = AbilityWeights[GetWeightedRandomIndex(AbilityWeights)].Ability;
        if (CheckHpGates() && BattleManager.instance.npcChars.Count < 2) return AbilityWeights[4].Ability;
        if (hp <= MaxHp * 0.5f)
        {
            AbilityWeights[0].Weight = 25;
            AbilityWeights[1].Weight = 25;
            AbilityWeights[2].Weight = 25;
            AbilityWeights[3].Weight = 25;
        }
        return chosenAbility;
    }
}
