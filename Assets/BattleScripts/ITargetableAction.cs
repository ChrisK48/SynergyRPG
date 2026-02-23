using UnityEngine;
using System.Collections.Generic;

public interface ITargetableAction
{
    string Name { get; }
    TargetType Targets { get; }
    void PerformAction(CharBattle user, List<CharBattle> targets);
}
