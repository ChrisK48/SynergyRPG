using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[Serializable]
public class RandomAbilityEffect : AbilityEffect
{
    [SerializeField] public List<AbilityWeight> abilityWeights;
    public override void ApplyEffect(CharBattle[] users, ITurnEntity target, int calculatedPower)
    {
        Ability selectedAbility = abilityWeights[GetWeightedRandomIndex()].Ability;
        selectedAbility.PerformAction(users, GetTargets(users, selectedAbility));
    }

    private int GetWeightedRandomIndex()
    {
        int totalWeight = 0;
        foreach (var w in abilityWeights) totalWeight += w.Weight;

        int roll = UnityEngine.Random.Range(0, totalWeight);
        int cursor = 0;

        for (int i = 0; i < abilityWeights.Count; i++)
        {
            cursor += abilityWeights[i].Weight;
            if (roll < cursor) return i;
        }
        return 0;
    }

    private List<ITurnEntity> GetTargets(CharBattle[] users, Ability selectedAbility)
    {
        List<ITurnEntity> targets = new List<ITurnEntity>();
        var activeStances = BattleManager.instance.GetSynergyStances();
        switch (selectedAbility.TargetType)
        {
            case TargetType.SingleEnemy:
                Debug.Log("Random Effect doesn't support singleEnemy");
                break;
            case TargetType.SingleAlly:
                Debug.Log("Random Effect doesn't support singleAlly");
                break;
            case TargetType.AllEnemies:
                targets.AddRange(BattleManager.instance.npcChars);
                break;
            case TargetType.AllAllies:
                targets.AddRange(BattleManager.instance.playerChars);
                break;
            case TargetType.Self:
                targets.AddRange(users);
                break;
            case TargetType.AnyChar:
                Debug.Log("Random Effect doesn't support anyChar");
                break;
            case TargetType.AllChars:
                targets.AddRange(BattleManager.instance.playerChars);
                targets.AddRange(BattleManager.instance.npcChars);
                break;
            case TargetType.DeadAlly:
                Debug.Log("Random Effect doesn't support deadAlly");
                break;
            case TargetType.DeadAllies:
                targets.AddRange(BattleManager.instance.playerChars.Where(pc => !pc.GetIfAlive()));
                break;
            case TargetType.RandomEnemies:
                targets.AddRange(BattleManager.instance.npcChars.Where(npc => npc.GetIfAlive() && !activeStances.Any(stance => stance.users.Contains(npc))));
                break;
            case TargetType.RandomAllies:
                targets.AddRange(BattleManager.instance.playerChars.Where(pc => pc.GetIfAlive() && !activeStances.Any(stance => stance.users.Contains(pc))));
                targets.AddRange(BattleManager.instance.GetSynergyStances());
                break;
        }
        return targets;
    }
}
