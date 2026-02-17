using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class NpcBattle : CharBattle
{
    public abstract void PerformAITurn();

    public abstract CharBattle NpcTargetingLogic(Ability ability);
}
