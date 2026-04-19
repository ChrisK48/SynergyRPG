using System;
using UnityEngine;
using System.Linq;
using UnityEditor;
using System.Collections.Generic;
using Unity.VisualScripting;

public class SynergyStance : ITurnEntity
{
    public CharBattle[] users;
    private string synergyName;
    public string EntityName => synergyName;
    private int amalgamatedSpd;
    public int spd => amalgamatedSpd;
    private bool isPreppingSynergy = false;
    public bool entityIsPreppingSynergy => isPreppingSynergy;
    private List<SynergyTag> storedTags = new List<SynergyTag>();
    private bool isDefending = false;
    private bool isHiding = false;
    private SynergyAbility storedAbility;
    private List<ITurnEntity> storedTargets;
    public bool entityDefending => isDefending;
    public bool hasActed { get; private set; }
    private ShieldCharBattle shieldPlayer;

    public SynergyStance(CharBattle[] users)
    {
        this.users = users;
        foreach (var user in users) user.EnterSynergyStance(this);
        synergyName = string.Join(" & ", users.Select(u => u.CharName)) + " Synergy Stance";
        amalgamatedSpd = (int)users.Average(u => u.Spd);
        shieldPlayer = users.OfType<ShieldCharBattle>().FirstOrDefault();
        shieldPlayer?.ActivateGuardianShield();
    }

    public void Defend()
    {
        isDefending = !isDefending;
        if (isDefending)
        {
            foreach (var user in users)
            {
                user.Defend();
            }
        }
    }
    
    public bool GetIfDefending() => isDefending;

    public void ReceiveBuff(Buff buff, int duration)
    {
        foreach (var user in users)
        {
            user.ReceiveBuff(buff, duration);
        }
    }

    public void ProcessTurnBuffs()
    {
        foreach (var user in users)
        {
            user.ProcessTurnBuffs();
        }
    }

    public void TakeDamage(int amt, AtkType atkType, List<DamageType> elementTypes, int shieldsToRemove = 0, bool ignoreDef = false, System.Action<int> onDamageDealt = null)
    {
        if (shieldPlayer != null && shieldPlayer.getShieldStatus)
        {
            shieldPlayer.HandleGuardianShieldHit();
            return;
        } 

        foreach (var user in users)
        {
            user.TakeDamage(amt, atkType, elementTypes, 0 ,ignoreDef, onDamageDealt);
        }
    }

    public void Heal(int amt)
    {
        foreach (var user in users)
        {
            user.Heal(amt);
        }
    }

    public void EndTurn()
    {
        SetHasActed(true);
        FlowManager.instance.ConsumeFlow(10); // Arbitrary flow cost for taking a turn in synergy stance, can adjust later
        ProcessTurnBuffs();
        BattleManager.instance.GetTurnManager().AdvanceTurn();
        BattleManager.instance.NextTurn();
        if (FlowManager.instance.currentFlow <= 0) 
        {
            Debug.Log("Synergy Stance broken due to flow depletion!");
            if (!isHiding) SynergyStanceManager.instance.EndSynergyStance(this);
        }
    }

    public void SetHasActed(bool value)
    {
        foreach (var user in users)
        {
            user.SetHasActed(value);
        }
    }

    public void StartPrep(List<SynergyTag> tags)
    {
        isPreppingSynergy = true;
    }

    public void EndPrep()
    {
        isPreppingSynergy = false;
        storedTags.Clear();
    }

    public void StoreTags(List<SynergyTag> tags) => storedTags.AddRange(tags);

    public bool GetIfPreppingSynergy() => isPreppingSynergy;
    public bool GetIfHiding() => isHiding;
    public void RevealChar()
    {
        Debug.Log("Stored move in stance triggered");
        isHiding = false;
        foreach(PlayerCharBattle user in users)
        {
            user.GetComponentInChildren<SpriteRenderer>().enabled = true;
        }
        foreach(var target in storedTargets)
        {
            storedAbility.ExecuteSynergy(users, target);
        }
        storedTargets = null;
        storedAbility = null;
        if (FlowManager.instance.currentFlow <= 0) SynergyStanceManager.instance.EndSynergyStance(this);
    }

    public void HideChar()
    {
        isHiding = true;
        foreach (PlayerCharBattle user in users)
        {
            user.GetComponentInChildren<SpriteRenderer>().enabled = false;
        }
    }

    public void StoreAbilityForNextTurn(SynergyAbility synergyAbility) => storedAbility = synergyAbility;
    public void StoreTargetsForNextTurn(List<ITurnEntity> targets) => storedTargets = targets;
}
