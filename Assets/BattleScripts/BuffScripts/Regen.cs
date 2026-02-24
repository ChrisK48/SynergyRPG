using UnityEngine;

[CreateAssetMenu(fileName = "New Regen Buff", menuName = "Buffs/Regen Buff")]
public class Regen : Buff
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
