using UnityEngine;
using System.Collections.Generic;

public class TheBoggedAI : NpcBattle
{
    public override Ability NpcAbilitySelection()
    {
        int randomIndex = Random.Range(0, 100);
            if (30 <= randomIndex && randomIndex < 70)
            {
                return AbilityWeights[0].Ability; // Quick swing
            }
            else if (randomIndex < 80)
            {
                return AbilityWeights[1].Ability; // Sink and Strike
            }
            else if (randomIndex < 95)
            {
                return AbilityWeights[2].Ability; // Thrash
            }
            else if (10 <= randomIndex && randomIndex < 30)
            {
                return AbilityWeights[3].Ability; // Mudshot
            }
            else if (randomIndex < 10)
            {
                return AbilityWeights[4].Ability; // Spawn Minions
            }
        return AbilityWeights[randomIndex].Ability;
    }
}
