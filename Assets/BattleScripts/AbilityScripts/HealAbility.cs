using UnityEngine;

[CreateAssetMenu(fileName = "New Heal Ability", menuName = "Abilities/Heal Ability")]
public class HealAbility : Ability
{
    protected override void ApplyEffect(CharBattle user, CharBattle target)
    {
        target.Heal(scalingMultiplier * GetUserStat(user));
    }
}