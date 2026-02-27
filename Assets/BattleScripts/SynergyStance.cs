using System;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class SynergyStance : ITurnEntity
{
    public CharBattle[] users;
    private string synergyName;
    public string entityName => synergyName;
    private int amalgamatedSpd;
    public int spd => amalgamatedSpd;
    private bool isPreppingSynergy = false;
    public bool entityIsPreppingSynergy => isPreppingSynergy;

    public SynergyStance(CharBattle[] users)
    {
        this.users = users;
        synergyName = string.Join(" & ", users.Select(u => u.CharName)) + " Synergy Stance";
        amalgamatedSpd = (int)users.Average(u => u.Spd);
    }

    public void ProcessTurnBuffs()
    {
        foreach (var user in users)
        {
            user.ProcessTurnBuffs();
        }
    }

    public void TakeDamage(int amt, AtkType atkType, bool ignoreDef = false, Action<int> onDamageDealt = null)
    {
        foreach (var user in users)
        {
            user.TakeDamage(amt, atkType, ignoreDef, onDamageDealt);
        }
    }

    public bool GetIfPreppingSynergy() => isPreppingSynergy;

    public void StartPrep(Ability[] abilities)
    {
        users[0].StorePreppedAbility(abilities[0]);
        users[1].StorePreppedAbility(abilities[1]);
        Debug.Log($"Synergy Stance prep started with abilities: {users[0].CharName} using {abilities[0].Name} and {users[1].CharName} using {abilities[1].Name}");
        isPreppingSynergy = true;
    }

    public void EndPrep()
    {
        isPreppingSynergy = false;
    }
}
