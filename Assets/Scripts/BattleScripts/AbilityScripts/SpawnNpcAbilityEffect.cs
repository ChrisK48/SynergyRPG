using UnityEngine;

[System.Serializable]
public class SpawnNpcAbilityEffect : AbilityEffect
{
    public NpcBattle NpcToSpawn;
    public int NumNpcToSpawn;
    public override void ApplyEffect(CharBattle[] users, ITurnEntity target, int calculatedPower)
    {
        for(int i = 0; i < NumNpcToSpawn; i++) BattleManager.instance.SpawnNpc(NpcToSpawn);
    }
}
