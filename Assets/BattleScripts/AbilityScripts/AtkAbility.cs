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
        int damage = calculateDamage(user);
        target.TakeDamage(damage);
    }

    public int calculateDamage(CharBattle user)
    {
        // Placeholder for more complex damage calculations based on attack type, elements, etc.
        return user.Atk * dmgMultiplier;
    }
}
