using UnityEngine;
using System.Collections.Generic;
public class SynergyDefend : ITargetableAction
{
    public string Name => "Synergy Defend";
    public CharBattle[] users;
    public TargetType Targets => TargetType.Self;

    public SynergyDefend(CharBattle[] users)
    {
        this.users = users;
    }

    public void PerformAction(CharBattle[] user, List<CharBattle> targets)
    {
        foreach (var member in users)
        {
            Debug.Log($"{member.CharName} defense before: {member.Def}");
            member.abilities[1].ExecuteAbility(member, member);
            Debug.Log($"{member.CharName} defense after: {member.Def}");
        }
    }
}
