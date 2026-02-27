using UnityEngine;

[System.Serializable]
public class ItemBuffEffect : ItemEffect
{
    public Buff buff;
    public int duration;
    public override void Apply(CharBattle user, CharBattle target) => target.ReceiveBuff(buff, duration);
}
