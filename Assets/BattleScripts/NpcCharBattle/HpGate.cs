using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HpGate
{
    public int Threshold;
    [HideInInspector]
    public bool HasTriggered = false;
}
