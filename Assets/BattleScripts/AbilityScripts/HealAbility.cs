using UnityEngine;

[CreateAssetMenu(fileName = "New Heal Ability", menuName = "Abilities/Heal Ability")]
public class HealAbility : Ability
{
    public override void ApplyEffect(CharBattle user, CharBattle target)
    {
        target.Heal(scalingMultiplier * GetUserStat(user));
    }
}