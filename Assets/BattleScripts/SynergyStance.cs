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
    private Ability[] preppedAbilities;
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

    public bool isPrepingSynergy() => isPreppingSynergy;
    public Ability[] getPreppedAbilities() => preppedAbilities;

    public void StartPrep(Ability[] abilities)
    {
        isPreppingSynergy = true;
        preppedAbilities = abilities;
    }

    public void EndPrep()
    {
        isPreppingSynergy = false;
        preppedAbilities = null;
    }
}
