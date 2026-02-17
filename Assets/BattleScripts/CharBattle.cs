using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Accessibility;

public abstract class CharBattle : MonoBehaviour
{
    BattleUIManager battleUIManager;
    public string charName;
    public int maxHp, maxMp, Hp, Mp, Atk, Matk, Def, Mdef, Spd, Luck;
    public bool isAlive = true;
    public List<Ability> abilities;
    public List<Buff> activeBuffs;

    void Awake()
    {
        battleUIManager = BattleUIManager.instance;
    }

    public virtual void Heal(int amt) {
        Hp = Mathf.Clamp(Hp + amt, 0, maxHp);
    }

    public virtual void TakeDamage(int amt)
    {
        // temp damage calculation
        Debug.Log(charName + " takes " + amt + " damage.");
        Hp = Mathf.Clamp(Hp - amt, 0, maxHp);
            if (Hp < 0)
                isAlive = false;
    }

    public void Die()
    {
        Debug.Log(charName + " has died.");
        isAlive = false;
    }

    public abstract void PerformAbility(Ability ability, List<CharBattle> targets);

    public void ReceiveBuff(Buff buff)
    {
        Buff instance = Instantiate(buff);
        foreach(Buff active in activeBuffs)
        {
            if(active.buffName == instance.buffName)
            {
                active.duration = Mathf.Max(active.duration, instance.duration);
                Debug.Log(charName + "'s " + buff.buffName + " buff duration refreshed to " + buff.duration + " turns.");
                return;
            }
        }
        activeBuffs.Add(instance);
        instance.StartBuff(this);
    }
}
