using UnityEngine;

[System.Serializable]
public class HealEffect : ItemEffect
{
    public int amount;
    public override void Apply(CharBattle user, CharBattle target) => target.Heal(amount);
}
