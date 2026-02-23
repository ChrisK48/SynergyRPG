using UnityEngine;

public abstract class ItemEffect : ScriptableObject
{
    public abstract void ApplyEffect(CharBattle user, CharBattle target);
}
