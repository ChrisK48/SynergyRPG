using System.Collections.Generic;
using UnityEngine;
public enum AtkType { Physical, Magical }
public enum ScalingStat { Atk, Mag, Def, Mdef, Spd, Acc, Eva, Luck }
public enum ElementType { Fire, Ice, Lightning, Water, Wind, Earth, Light, Void, None }
public enum TargetType { SingleEnemy, AllEnemies, Self, SingleAlly, AllAllies, AnyChar, AllChars }

[CreateAssetMenu(fileName = "New Ability", menuName = "Ability")]
public class Ability : ScriptableObject, ITargetableAction
{
    public string abilityName;
    public string Name => abilityName;
    public string description;
    public int mpCost;
    public int hpCost;
    [SerializeReference]
    public UniqueResourceCost uniqueResourceCost;
    public TargetType targetType;
    public TargetType Targets => targetType;
    [SerializeReference]
    public List<AbilityEffect> abilityEffects = new List<AbilityEffect>();

    public virtual void ExecuteAbility(CharBattle user, CharBattle target)
    {
        foreach (AbilityEffect effect in abilityEffects)
        {
            effect.ExecuteEffect(new CharBattle[] {user}, target);
        }
    }

    public void PerformAction(CharBattle[] user, List<CharBattle> targets)
    {
        if (user[0] is PlayerCharBattle player)
        {
            if (!player.CanPerformAbility(this, targets)) return;
        }
        
        foreach (CharBattle target in targets)
        {
            ExecuteAbility(user[0], target);
        }
    }
}