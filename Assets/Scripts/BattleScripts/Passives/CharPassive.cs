using UnityEngine;

public enum PassiveTrigger { OnTurnStart, OnTurnEnd, OnHit, OnDamageTaken, OnBlocked, OnHeal, OnLethalHit, OnDeath, PassiveTriggerCount }

[CreateAssetMenu(fileName = "New Passive", menuName = "Passive")]
public class CharPassive : ScriptableObject
{
    public string passiveName;
    public string passiveDescription;
    public PassiveTrigger trigger;
    public Sprite passiveSprite;
    [SerializeReference]
    public PassiveEffect passiveEffect;

    public void ActivatePassive(CharBattle user, int damage)
    {
        if (passiveEffect != null)
        {
            passiveEffect.ApplyEffect(user, damage);
        }
    }
}
