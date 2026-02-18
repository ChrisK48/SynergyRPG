using UnityEngine;
using System.Collections.Generic;

public class CaveBatBattle : NpcBattle
{

    public override Ability NpcAbilitySelectionLogic()
    {
        int randomInt = Random.Range(0, 100);
        if (Hp < maxHp * 0.5 && randomInt < 50)
        {
            return abilities[1]; // Drain Attack
        }
        else
        {
            return abilities[0]; // Regular Attack
        }
    }  

    public override CharBattle NpcTargetingLogic(Ability ability)
    {
        int randomIndex = Random.Range(0, BattleManager.instance.alivePlayerChars.Count);
        CharBattle target = BattleManager.instance.alivePlayerChars[randomIndex];
        return target;
    }
}
