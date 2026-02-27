using UnityEngine;

[System.Serializable]
public class CountDown : BuffEffect
{
    public override void EndBuff(CharBattle target, ActiveBuff buffWrapper)
    {
        Debug.Log(target.CharName + "'s CountDown buff has ended." + target.CharName + " is instantly killed!");
        target.TakeDamage(999999, AtkType.Magical, true);
        base.EndBuff(target, buffWrapper);
    }
}
