using UnityEngine;

[System.Serializable]
public class Poisoned : BuffEffect
{
    public int poisonDamage;
    public override void TickBuff(CharBattle target, ActiveBuff buffWrapper)
    {
        base.TickBuff(target, buffWrapper);
        target.TakeDamage(poisonDamage, AtkType.Physical, true);
    }
}
