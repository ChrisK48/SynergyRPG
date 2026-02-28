using System.Collections.Generic;
using UnityEngine;
public enum AtkType { Physical, Magical }
public enum ScalingStat { Atk, Mag, Def, Mdef, Spd, Acc, Eva, Luck }
public enum ElementType { Fire, Ice, Lightning, Water, Wind, Earth, Light, Void, None }
public enum TargetType { SingleEnemy, AllEnemies, Self, SingleAlly, AllAllies, AnyChar, AllChars, DeadAlly, DeadAllies }

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
    [SerializeReference]
    public UniqueResourceCost UniqueResourceCost;
    public TargetType TargetType;
    public TargetType Targets => TargetType;
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

    public void PerformAction(CharBattle[] user, List<ITurnEntity> targets)
    {
        if (user[0] is PlayerCharBattle player)
        {
            if (!player.CanPerformAbility(this)) return;
        }
        
        foreach (ITurnEntity target in targets)
        {
            ExecuteAbility(user[0], target);
        }
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
}