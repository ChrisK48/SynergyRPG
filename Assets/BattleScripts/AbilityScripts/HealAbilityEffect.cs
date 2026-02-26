using UnityEngine;

[System.Serializable]
public class HealAbilityEffect : AbilityEffect
{
    public override void ApplyEffect(CharBattle[] users, CharBattle target)
    {
        // Temporary formula for healing based on user's stat and scaling multiplier
        target.Heal(scalingMultiplier * GetUserStat(users));
    }
}