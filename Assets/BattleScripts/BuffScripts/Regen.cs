using UnityEngine;

[System.Serializable]
public class Regen : BuffEffect
{
    public int regenAmount;

    public override void StartBuff(CharBattle target, ActiveBuff buffWrapper)
    {
        Debug.Log(target.charName + " has started Regen buff.");
    }

    public override void TickBuff(CharBattle target, ActiveBuff buffWrapper)
    {
        target.Heal(regenAmount);
        Debug.Log(target.charName + " regenerates " + regenAmount + " HP.");
    }

    public override void EndBuff(CharBattle target, ActiveBuff buffWrapper)
    {
        Debug.Log(target.charName + "'s Regen buff has ended.");
    }
}
