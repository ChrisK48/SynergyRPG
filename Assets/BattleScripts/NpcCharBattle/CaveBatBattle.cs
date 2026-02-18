using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
        List<PlayerCharBattle> alivePlayers = BattleManager.instance.playerChars.Where(pc => pc.isAlive).ToList();
        int randomIndex = Random.Range(0, alivePlayers.Count());
        CharBattle target = alivePlayers[randomIndex];
        return target;
    }
}
