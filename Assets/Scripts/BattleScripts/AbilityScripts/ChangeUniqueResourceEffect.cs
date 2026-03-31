using System;
using UnityEngine;

[Serializable]
public class ChangeUniqueResourceEffect : AbilityEffect
{
    public int resourceChangeAmount;
    public UniqueResource resourceType;
    public override void ApplyEffect(CharBattle[] users, ITurnEntity target, int calculatedPower)
    {
        foreach (var user in users)
        {
            switch (resourceType)
            {
                case UniqueResource.Shield:
                    if (target is ShieldCharBattle shieldTarget)
                    {
                        if (resourceChangeAmount > 0) shieldTarget.GainShield(resourceChangeAmount);
                        else shieldTarget.LoseShield(-resourceChangeAmount);
                    }
                    return;
                case UniqueResource.Void:
                    if (target is VoidCharBattle voidTarget)
                    {
                        if (resourceChangeAmount > 0) voidTarget.GainVoid(resourceChangeAmount);
                        else voidTarget.LoseVoid(-resourceChangeAmount);
                    }
                    return;
                default:
                    Debug.LogWarning("Unhandled unique resource type in ChangeUniqueResourceEffect");
                    return;
            }
        }
    }
}
