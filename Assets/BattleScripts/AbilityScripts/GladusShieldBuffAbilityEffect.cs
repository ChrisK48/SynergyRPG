using System;

[Serializable]
public class GladusShieldBuffAbilityEffect : BuffAbilityEffect
{
    public override void ApplyEffect(CharBattle[] users, ITurnEntity target, int calculatedPower)
    {
        ShieldCharBattle shieldUser = (ShieldCharBattle)System.Array.Find(users, u => u is ShieldCharBattle);
        if (shieldUser != null) buffDuration = shieldUser.GetShieldPoints() / 2;
        base.ApplyEffect(users, target, calculatedPower);
    }
}
