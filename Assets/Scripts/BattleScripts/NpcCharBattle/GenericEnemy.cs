using UnityEngine;

public class GenericEnemy : NpcBattle
{
    public override Ability NpcAbilitySelection()
    {
        return AbilityWeights[GetWeightedRandomIndex(AbilityWeights)].Ability;
    }
}
