using UnityEngine;
using System.Collections.Generic;

public class SynergyAttack : ITargetableAction
{
    public string Name => "Synergy Attack";
    public CharBattle[] users;
    public TargetType Targets => TargetType.AnyChar;

    public SynergyAttack(CharBattle[] users)
    {
        this.users = users;
    }

    public void PerformAction(CharBattle[] user, List<ITurnEntity> targets)
    {
        foreach (PlayerCharBattle member in users)
        {
            member.abilities[0].PerformAction(new CharBattle[] { member }, targets);
        }
    }
}
