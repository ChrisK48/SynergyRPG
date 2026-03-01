using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Accessibility;

public abstract class CharBattle : MonoBehaviour, ITurnEntity
{
    BattleUIManager battleUIManager;
    public string CharName;
    public string EntityName => CharName;
    public int MaxHp, MaxMp, Atk, Mag, Def, Mdef, Spd, Acc, Eva, Luck;
    protected int hp, mp;
    public int spd => Spd;
    protected bool isAlive = true;
    public List<Ability> abilities;
    public List<ActiveBuff> activeBuffs;
    protected bool isPreppingSynergy = false;
    protected bool inSynergyStance = false;
    protected SynergyStance currentSynergyStance;
    public bool entityIsPreppingSynergy => isPreppingSynergy;
    protected Ability preppedAbility;

    void Awake()
    {
        battleUIManager = BattleUIManager.instance;
        hp = MaxHp;
        mp = MaxMp;
    }

    public int getHp() => hp;
    public int getMp() => mp;

    public virtual void Heal(int amt) {
        if (hp == 0) return;
        Debug.Log(CharName + " heals for " + amt + " HP.");
        hp = Mathf.Clamp(hp + amt, 0, MaxHp);
        BattleUIManager.instance.Popup(amt, transform.position, PopupType.Heal);
    }

    public virtual void TakeDamage(int amt, AtkType atkType, List<DamageType> elementTypes = null, int shieldsToRemove = 0, bool ignoreDef = false, System.Action<int> onDamageDealt = null)
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

        int finalDamage = Mathf.Min(damage, hp);
        Debug.Log(CharName + " takes " + finalDamage + " damage.");

        hp = Mathf.Clamp(hp - finalDamage, 0, MaxHp);
            if (hp == 0)
                Die();

        onDamageDealt?.Invoke(finalDamage);
        BattleUIManager.instance.Popup(finalDamage, transform.position, PopupType.Damage);
    }

    public bool GetIfAlive() => isAlive;

    protected virtual void Die()
    {
        Debug.Log(CharName + " has died.");
        isAlive = false;
        if (inSynergyStance) SynergyStanceManager.instance.BreakSynergyStance(currentSynergyStance);
        BattleManager.instance.GetTurnManager().RemoveFromTurnOrder(this);
    }

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

    // Used on prep to store the prepped ability so that resource costs can be deducted on the next turn when the synergy is executed
    public void StartPrep(Ability[] abilities)
    {
        isPreppingSynergy = true;
        preppedAbility = abilities[0];
        Debug.Log(CharName + " has started prepping " + preppedAbility.Name);
    }

    public bool IsPreppingSynergy() => isPreppingSynergy;
    public bool GetIfInSynergyStance() => inSynergyStance;
    public Ability GetPreppedAbility() => preppedAbility;

    public void StorePreppedAbility(Ability ability)
    {
        preppedAbility = ability;
    }

    public void EndPrep()
    {
        isPreppingSynergy = false;
        preppedAbility = null;
    }

    public void EnterSynergyStance(SynergyStance stance)
    {
        inSynergyStance = true;
        currentSynergyStance = stance;
    }

    public void ExitSynergyStance()
    {
        inSynergyStance = false;
        currentSynergyStance = null;
    }
}
