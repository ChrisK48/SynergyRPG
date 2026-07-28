using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class DeflectDamagePassive : PassiveEffect
{
    public override void ApplyEffect(CharBattle user, int damage)
    {

        if (user is PlayerCharBattle) 
        {
            List<PlayerCharBattle> validTargets = BattleManager.instance.playerChars.Where(c => c != user).ToList();
            int randomIndex = Random.Range(0, validTargets.Count);
            Debug.Log(user.CharName + "'s Deflect Damage passive activates, redirecting damage to " + validTargets[randomIndex].CharName);
            validTargets[randomIndex].TakeDamage(damage * 2, AtkType.Magical, new List<DamageType>(), 0, true);
            user.setHp(1);
        }
    }
}
