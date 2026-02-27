using UnityEngine;

[System.Serializable]
public class ItemReviveEffect : ItemEffect
{
    public int HealAmount;
    public override void Apply(CharBattle user, CharBattle target)
    {
        if (target is PlayerCharBattle pc)
        {
            pc.Revive();
            pc.Heal(HealAmount - 1);
        }
    }
}
