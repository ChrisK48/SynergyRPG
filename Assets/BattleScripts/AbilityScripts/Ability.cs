using UnityEngine;
public enum AtkType { Physical, Magical }
public enum ElementType { Fire, Ice, Lightning, Water, Earth, Light, Void, None }
public enum TargetType { SingleEnemy, AllEnemies, Self, SingleAlly, AllAllies, AnyChar, AllChars }

public abstract class Ability : ScriptableObject
{
    public string abilityName;
    public string description;
    public int mpCost;
    public int hpCost;
    public int uniqueCost;
    public ElementType elementType;
    public TargetType targetType;

    public virtual void ExecuteAbility(CharBattle user, CharBattle target)
    {
    }
}
