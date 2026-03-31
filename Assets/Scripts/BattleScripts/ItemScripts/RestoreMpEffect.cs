using UnityEngine;

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
