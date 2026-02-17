using UnityEngine;

[CreateAssetMenu(fileName = "New Atk Ability", menuName = "Abilities/Attack Ability")]
public class AtkAbility : Ability
{
    public int dmgMultiplier;
    public bool isMissable;
    public AtkType atkType;

    public override void ExecuteAbility(CharBattle user, CharBattle target)
    {
        base.ExecuteAbility(user, target);
        int damage = user.Atk * dmgMultiplier;
        target.TakeDamage(damage);
    }
}
