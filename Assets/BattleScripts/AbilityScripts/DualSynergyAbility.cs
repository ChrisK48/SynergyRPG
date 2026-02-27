using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "New DualSynergy", menuName = "Synergies/DualSynergyAbility")]
public class DualSynergyAbility : SynergyAbility, ITargetableAction
{
    public List<SynergyTagSet> synergyTagSets = new List<SynergyTagSet>();
}
