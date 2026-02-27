using UnityEngine;

[System.Serializable]
public abstract class ItemEffect 
{
    public abstract void Apply(CharBattle user, CharBattle target);
}