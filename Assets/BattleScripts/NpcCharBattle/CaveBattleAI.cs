using UnityEngine;

public class CaveBatBattle : NpcBattle
{

    public override Ability NpcAbilitySelection()
    {
        int randomInt = Random.Range(0, 100);
        if (hp < MaxHp * 0.5 && randomInt < 50)
        {
            return AbilityWeights[1].Ability; // Drain Attack
        }
        else
        {
            return AbilityWeights[0].Ability; // Regular Attack
        }
    }  
}
