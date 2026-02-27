using UnityEngine;

[System.Serializable]
public class ItemAbilityEffect : ItemEffect
{
    public Ability ability;
    public override void Apply(CharBattle user, CharBattle target) => ability.ExecuteAbility(user, target);
}
