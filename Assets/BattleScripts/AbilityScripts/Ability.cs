using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public enum AtkType { Physical, Magical }
public enum ScalingStat { Atk, Mag, Def, Mdef, Spd, Acc, Eva, Luck }
public enum TargetType { SingleEnemy, AllEnemies, Self, SingleAlly, AllAllies, AnyChar, AllChars, DeadAlly, DeadAllies, RandomEnemies, RandomAllies }

[CreateAssetMenu(fileName = "New Ability", menuName = "Ability")]
public class Ability : ScriptableObject, ITargetableAction
{
    public string AbilityName;
    public string Name => AbilityName;
    public string Description;
    public ScalingStat ScalingStat; 
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

        if (user[0] is PlayerCharBattle player)
        {
            if (!player.CanPerformAbility(this)) return;
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
        if (ScalingStat == ScalingStat.Atk)
            calculatedPower = ScalingMultiplier * user.Atk;
        else if (ScalingStat == ScalingStat.Mag)
            calculatedPower = ScalingMultiplier * user.Mag;
        else if (ScalingStat == ScalingStat.Def)
            calculatedPower = ScalingMultiplier * user.Def;
        else if (ScalingStat == ScalingStat.Mdef)
            calculatedPower = ScalingMultiplier * user.Mdef;
        else if (ScalingStat == ScalingStat.Spd)
            calculatedPower = ScalingMultiplier * user.Spd;
        else if (ScalingStat == ScalingStat.Acc)
            calculatedPower = ScalingMultiplier * user.Acc;
        else if (ScalingStat == ScalingStat.Eva)
            calculatedPower = ScalingMultiplier * user.Eva;
        else if (ScalingStat == ScalingStat.Luck)
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