using UnityEngine;

[CreateAssetMenu(fileName = "New Buff Ability", menuName = "Abilities/Buff Ability")]
public class BuffAbility : Ability
{
    public Buff buffToApply;

    public override void ExecuteAbility(CharBattle user, CharBattle target)
    {
        base.ExecuteAbility(user, target);
        target.ReceiveBuff(buffToApply);   
    }
}
