using UnityEngine;
using System.Collections.Generic;

public class TheBoggedAI : NpcBattle
{
    public override Ability NpcAbilitySelectionLogic()
    {
        int randomIndex = Random.Range(0, abilities.Count);
        return abilities[randomIndex];
    }

    public override ITurnEntity NpcTargetingLogic(Ability ability)
    {
        List<ITurnEntity> alivePlayers = GetPotentialTargets();
        int randomIndex = Random.Range(0, alivePlayers.Count);
        ITurnEntity target = alivePlayers[randomIndex];
        return target;
    }
}
