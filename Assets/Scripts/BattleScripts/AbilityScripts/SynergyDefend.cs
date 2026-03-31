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

    public void PerformAction(CharBattle[] user, List<ITurnEntity> targets, System.Action onComplete = null)
    {
        foreach (PlayerCharBattle member in users)
        {
            member.Defend();
        }
        onComplete?.Invoke();
    }
}
