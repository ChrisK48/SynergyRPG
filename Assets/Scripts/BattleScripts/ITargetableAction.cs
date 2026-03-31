using UnityEngine;
using System.Collections.Generic;

public interface ITargetableAction
{
    string Name { get; }
    TargetType Targets { get; }
    void PerformAction(CharBattle[] users, List<ITurnEntity> targets, System.Action onComplete = null);
}
