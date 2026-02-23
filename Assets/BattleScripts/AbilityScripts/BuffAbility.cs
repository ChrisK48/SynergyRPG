using UnityEngine;

[CreateAssetMenu(fileName = "New Buff Ability", menuName = "Abilities/Buff Ability")]
public class BuffAbility : Ability
{
    public Buff buffToApply;

    protected override void ApplyEffect(CharBattle user, CharBattle target)
    {
        target.ReceiveBuff(buffToApply);   
    }
}
