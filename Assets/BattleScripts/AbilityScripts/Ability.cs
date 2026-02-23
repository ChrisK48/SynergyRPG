using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
public enum AtkType { Physical, Magical }
public enum ScalingStat { Atk, Matk, Def, Mdef, Spd, Luck }
public enum ElementType { Fire, Ice, Lightning, Water, Earth, Light, Void, None }
public enum TargetType { SingleEnemy, AllEnemies, Self, SingleAlly, AllAllies, AnyChar, AllChars }

public abstract class Ability : ScriptableObject, ITargetableAction
{
    public string abilityName;
    public string Name => abilityName;
    public string description;
    public int mpCost;
    public int hpCost;
    public int uniqueCost;
    public List<ElementType> elementTypes;
    public TargetType targetType;
    public TargetType Targets => targetType;
    public bool isMissable;
    public int hitChance;
    public ScalingStat scalingStat; 
    public int scalingMultiplier;

    public virtual void ExecuteAbility(CharBattle user, CharBattle target)
    {
        if (!CheckIfHit(user, target)) return;
        ApplyEffect(user, target);
    }

    protected abstract void ApplyEffect(CharBattle user, CharBattle target);

    protected bool CheckIfHit(CharBattle user, CharBattle target)
    {
        if (isMissable)
        {
            // temp hit calculation
            if (Random.value > (hitChance / 100f))
            {
                Debug.Log(user.charName + "'s " + abilityName + " missed!");
                return false;
            }
        }
        return true;
    }
    
    protected int GetUserStat(CharBattle user)
    {
        switch (scalingStat)
        {
            case ScalingStat.Atk:
                return user.Atk;
            case ScalingStat.Matk:
                return user.Matk;
            case ScalingStat.Def:
                return user.Def;
            case ScalingStat.Mdef:
                return user.Mdef;
            case ScalingStat.Spd:
                return user.Spd;
            case ScalingStat.Luck:
                return user.Luck;
            default:
                return 0;
        }
    }

    public void PerformAction(CharBattle user, List<CharBattle> targets)
    {
        user.PerformAction(this, targets);
    }
}
