using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Accessibility;

public abstract class CharBattle : MonoBehaviour
{
    BattleUIManager battleUIManager;
    public string charName;
    public int maxHp, maxMp, Hp, Mp, Atk, Mag, Def, Mdef, Spd, Acc, Eva, Luck;
    public bool isAlive = true;
    public List<Ability> abilities;
    public List<ActiveBuff> activeBuffs;

    void Awake()
    {
        battleUIManager = BattleUIManager.instance;
    }

    public virtual void Heal(int amt) {
        Debug.Log(charName + " heals for " + amt + " HP.");
        Hp = Mathf.Clamp(Hp + amt, 0, maxHp);
    }

    public virtual void TakeDamage(int amt, AtkType atkType, bool ignoreDef = false, System.Action<int> onDamageDealt = null)
    {
        // temp damage calculation
        int damage = 0;

        if (!ignoreDef)
            if (atkType == AtkType.Physical)
                damage = Mathf.Max(amt - Def, 1);
            else
                damage = Mathf.Max(amt - Mdef, 1);
        else
            damage = amt;

        int finalDamage = Mathf.Min(damage, Hp);
        Debug.Log(charName + " takes " + finalDamage + " damage.");

        Hp = Mathf.Clamp(Hp - finalDamage, 0, maxHp);
            if (Hp == 0)
                Die();

        onDamageDealt?.Invoke(finalDamage);

    }

    public virtual void Die()
    {
        Debug.Log(charName + " has died.");
        isAlive = false;
    }

    public abstract void PerformAction(ITargetableAction action, List<CharBattle> targets);
    public void ReceiveBuff(Buff buff, int duration)
    {

        ActiveBuff existing = activeBuffs.Find(b => b.buff == buff);
        if (existing != null)
        {
            existing.remainingDuration = Mathf.Max(existing.remainingDuration + duration);
            return;
        }

        ActiveBuff newBuff = new ActiveBuff(buff, duration);
        activeBuffs.Add(newBuff);
        buff.StartBuff(this, newBuff);
    }

    public void ProcessTurnBuffs()
    {
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            ActiveBuff active = activeBuffs[i];
            
            active.Tick(this);
            
            active.remainingDuration--;

            if (active.remainingDuration <= 0)
            {
                activeBuffs.RemoveAt(i);
            }
        }
    }
}
