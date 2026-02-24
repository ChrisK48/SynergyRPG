using UnityEngine;

[CreateAssetMenu(fileName = "New CountDown Buff", menuName = "Buffs/CountDown Buff")]
public class CountDown : Buff
{
    public override void EndBuff(CharBattle target, ActiveBuff buffWrapper)
    {
        Debug.Log(target.charName + "'s CountDown buff has ended." + target.charName + " is instantly killed!");
        target.TakeDamage(999999);
        base.EndBuff(target, buffWrapper);
    }
}
