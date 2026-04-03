using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using Microsoft.Unity.VisualStudio.Editor;
public enum AtkType { Physical, Magical }
public enum Stat { Atk, Mag, Def, Mdef, Spd, Acc, Eva, Luck, MaxHp, MaxMp, Hp, Mp }
public enum TargetType { SingleEnemy, AllEnemies, Self, SingleAlly, AllAllies, AnyChar, AllChars, DeadAlly, DeadAllies, RandomEnemies, RandomAllies }

[CreateAssetMenu(fileName = "New Ability", menuName = "Ability")]
public class Ability : ScriptableObject, ITargetableAction
{
    public string AbilityName;
    public string Name => AbilityName;
    public string Description;
    public Stat ScalingStat; 
    public int ScalingMultiplier;
    public int MpCost;
    public int HpCost;
    public bool SkipTurnAndUseNextTurn;
    [SerializeReference]
    public UniqueResourceCost UniqueResourceCost;
    public TargetType TargetType;
    public TargetType Targets => TargetType;
    public int TargetsToHitIfRandom;
    [SerializeReference]
    public List<AbilityEffect> AbilityEffects = new List<AbilityEffect>();
    public List<SynergyTag> SynergyTags = new List<SynergyTag>();

    public virtual void ExecuteAbility(CharBattle user, ITurnEntity target)
    {
        int calculatedPower = CalculatePower(user);
        foreach (AbilityEffect effect in AbilityEffects)
        {
            effect.ExecuteEffect(new CharBattle[] {user}, target, calculatedPower);
        }
    }

    public void PerformAction(CharBattle[] user, List<ITurnEntity> targets, System.Action onComplete = null)
    {
        if (SkipTurnAndUseNextTurn)
        {
            foreach(var entity in user)
            {
                entity.StoreAbilityForNextTurn(this);
                entity.StoreTargetsForNextTurn(targets);
                entity.HideChar();
            }
            onComplete?.Invoke();
            return;
        }

        if (TargetType == TargetType.RandomAllies || TargetType == TargetType.RandomEnemies) targets = GetRandomTargets(targets);
        for(int i = 0; i < targets.Count; i++)
        {
            ExecuteAbility(user[0], targets[i]);
        }
        onComplete?.Invoke();
    }

    private int CalculatePower(CharBattle user)
    {
        int calculatedPower = 0;
        if (ScalingStat == Stat.Atk)
            calculatedPower = ScalingMultiplier * user.Atk;
        else if (ScalingStat == Stat.Mag)
            calculatedPower = ScalingMultiplier * user.Mag;
        else if (ScalingStat == Stat.Def)
            calculatedPower = ScalingMultiplier * user.Def;
        else if (ScalingStat == Stat.Mdef)
            calculatedPower = ScalingMultiplier * user.Mdef;
        else if (ScalingStat == Stat.Spd)
            calculatedPower = ScalingMultiplier * user.Spd;
        else if (ScalingStat == Stat.Acc)
            calculatedPower = ScalingMultiplier * user.Acc;
        else if (ScalingStat == Stat.Eva)
            calculatedPower = ScalingMultiplier * user.Eva;
        else if (ScalingStat == Stat.Luck)
            calculatedPower = ScalingMultiplier * user.Luck;

        return calculatedPower;
    }

    private List<ITurnEntity> GetRandomTargets(List<ITurnEntity> targets)
    {
        List<ITurnEntity> randomTargets = new List<ITurnEntity>();
        for (int i = 0; i < TargetsToHitIfRandom; i++)
        {
            int randomIndex = Random.Range(0, targets.Count);
            randomTargets.Add(targets[randomIndex]);
        }
        return randomTargets;
    }
}