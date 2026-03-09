using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    public List<ActiveBuff> activeBuffs;
    protected bool isPreppingSynergy = false;
    protected bool inSynergyStance = false;
    protected SynergyStance currentSynergyStance;
    public bool entityIsPreppingSynergy => isPreppingSynergy;
    protected Ability preppedAbility;
    protected Ability storedAbility;
    protected List<ITurnEntity> storedTargets;
    protected int startDef;
    protected int startMdef;
    protected bool isDefending = false;
    protected bool isHiding = false;

    void Awake()
    {
        battleUIManager = BattleUIManager.instance;
        hp = MaxHp;
        mp = MaxMp;
        startDef = Def;
        startMdef = Mdef;
    }

    public int getHp() => hp;
    public int getMp() => mp;

    public virtual void Heal(int amt) {
        if (hp == 0) return;
        Debug.Log(CharName + " heals for " + amt + " HP.");
        hp = Mathf.Clamp(hp + amt, 0, MaxHp);
        BattleUIManager.instance.Popup(amt, transform.position, PopupType.Heal);
    }

    public virtual void Defend()
    {
        isDefending = !isDefending;
        if (isDefending)
        {
            Def = Mathf.RoundToInt(startDef * 1.25f);
            Mdef = Mathf.RoundToInt(startMdef * 1.25f);
            Debug.Log(CharName + " is defending! Def and Mdef increased.");
        }
        else
        {
            Def = startDef;
            Mdef = startMdef;
            Debug.Log(CharName + " stopped defending. Def and Mdef returned to normal.");
        }
    }

    public bool GetIfDefending() => isDefending;

    public virtual void TakeDamage(int amt, AtkType atkType, List<DamageType> elementTypes = null, int shieldsToRemove = 0, bool ignoreDef = false, System.Action<int> onDamageDealt = null)
    {
        // temp damage calculation
        int damage = 0;

        if (!ignoreDef)
            if (atkType == AtkType.Physical)
                damage = Mathf.Max(amt*amt/(amt+Def), 1);
            else
                damage = Mathf.Max(amt*amt/(amt+Mdef), 1);
        else
            damage = amt;

        int finalDamage = Mathf.Min(damage, hp);
        Debug.Log(CharName + " takes " + finalDamage + " damage.");

        hp = Mathf.Clamp(hp - finalDamage, 0, MaxHp);
            if (hp == 0)
                Die();

        onDamageDealt?.Invoke(finalDamage);
        BattleUIManager.instance.Popup(damage, transform.position, PopupType.Damage);
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
    public SynergyStance GetCurrentSynergyStance() => currentSynergyStance;
    public Ability GetPreppedAbility() => preppedAbility;
    public void StoreAbilityForNextTurn(Ability ability) => storedAbility = ability;
    public List<ITurnEntity> StoreTargetsForNextTurn(List<ITurnEntity> targets) => storedTargets = targets;
    public List<ITurnEntity> GetStoredTargets() => storedTargets;
    public void ClearStoredAbilityAndTargets()
    {
        storedAbility = null;
        storedTargets = null;
    }
    public Ability GetStoredAbility() => storedAbility;
    public bool GetIfHiding() => isHiding;
    public void HideChar()
    {
        isHiding = true;
        GetComponentInChildren<SpriteRenderer>().enabled = false;
    }

    public void RevealChar()
    {
        isHiding = false;
        GetComponentInChildren<SpriteRenderer>().enabled = true;
    }

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
