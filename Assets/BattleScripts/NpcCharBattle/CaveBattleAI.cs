using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CaveBatBattle : NpcBattle
{

    public override Ability NpcAbilitySelectionLogic()
    {
        int randomInt = Random.Range(0, 100);
        if (hp < MaxHp * 0.5 && randomInt < 50)
        {
            return abilities[1]; // Drain Attack
        }
        else
        {
            return abilities[0]; // Regular Attack
        }
    }  

    public override ITurnEntity NpcTargetingLogic(Ability ability)
    {
        List<ITurnEntity> alivePlayers = GetPotentialTargets();
        int randomIndex = Random.Range(0, alivePlayers.Count());
        ITurnEntity target = alivePlayers[randomIndex];
        return target;
    }
}
