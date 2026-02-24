using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public abstract class ItemEffect 
{
    public abstract void Apply(CharBattle user, CharBattle target);
}

[System.Serializable]
public class HealEffect : ItemEffect
{
    public int amount;
    public override void Apply(CharBattle user, CharBattle target) => target.Heal(amount);
}

[System.Serializable]
public class RestoreMPEffect : ItemEffect
{
    public int amount;
    public override void Apply(CharBattle user, CharBattle target)
    {
        if (target is PlayerCharBattle pc)
            pc.ChangeMp(amount);
    }
}

[System.Serializable]
public class AbilityEffect : ItemEffect
{
    public Ability ability;
    public override void Apply(CharBattle user, CharBattle target) => ability.ApplyEffect(user, target);
}

[System.Serializable]
public class BuffEffect : ItemEffect
{
    public Buff buff;
    public int duration;
    public override void Apply(CharBattle user, CharBattle target) => target.ReceiveBuff(buff, duration);
}