using UnityEngine;

[CreateAssetMenu(fileName = "New Regen Buff", menuName = "Buffs/Regen Buff")]
public class Regen : Buff
{
    public int regenAmount;

    public override void StartBuff(CharBattle target)
    {
        Debug.Log(target.charName + " has started Regen buff.");
    }

    public override void TickBuff(CharBattle target)
    {
        target.Heal(regenAmount);
        Debug.Log(target.charName + " regenerates " + regenAmount + " HP.");
        base.TickBuff(target);
    }

    public override void EndBuff(CharBattle target)
    {
        Debug.Log(target.charName + "'s Regen buff has ended.");
        target.activeBuffs.Remove(this);
    }
}
