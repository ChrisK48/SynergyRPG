using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DeflectDamagePassive : PassiveEffect
{
    public override void ApplyEffect(CharBattle user, int damage)
    {
        int total = BattleManager.instance.playerChars.Count;
        int randomIndex = Random.Range(0, total);
        Debug.Log(user.CharName + "'s Deflect Damage passive activates, redirecting damage to " + BattleManager.instance.playerChars[1].CharName);
        BattleManager.instance.playerChars[1].TakeDamage(damage * 2, AtkType.Magical, new List<DamageType>(), 0, true);
        user.setHp(1);
    }
}
