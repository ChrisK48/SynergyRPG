using System;
using UnityEngine;
using System.Linq;
using UnityEditor;
using System.Collections.Generic;

public class SynergyStance : ITurnEntity
{
    public CharBattle[] users;
    private string synergyName;
    public string EntityName => synergyName;
    private int amalgamatedSpd;
    public int spd => amalgamatedSpd;
    private bool isPreppingSynergy = false;
    public bool entityIsPreppingSynergy => isPreppingSynergy;
    private bool isDefending = false;
    public bool entityDefending => isDefending;

    public SynergyStance(CharBattle[] users)
    {
        this.users = users;
        foreach (var user in users) user.EnterSynergyStance(this);
        synergyName = string.Join(" & ", users.Select(u => u.CharName)) + " Synergy Stance";
        amalgamatedSpd = (int)users.Average(u => u.Spd);
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

    public bool GetIfPreppingSynergy() => isPreppingSynergy;

    public void StartPrep(Ability[] abilities)
    {
        users[0].StorePreppedAbility(abilities[0]);
        users[1].StorePreppedAbility(abilities[1]);
        Debug.Log($"Synergy Stance prep started with abilities: {users[0].CharName} using {abilities[0].Name} and {users[1].CharName} using {abilities[1].Name}");
        isPreppingSynergy = true;
        FlowManager.instance.ConsumeFlow(10);
    }

    public void EndPrep()
    {
        isPreppingSynergy = false;
    }

    public void EndTurn()
    {
        ProcessTurnBuffs();
        BattleManager.instance.GetTurnManager().AdvanceTurn();
        BattleManager.instance.NextTurn();
    }
}
