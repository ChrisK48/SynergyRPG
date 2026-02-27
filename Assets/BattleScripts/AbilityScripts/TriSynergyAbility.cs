using UnityEngine;

[CreateAssetMenu(fileName = "New TriSynergy", menuName = "Synergies/TriSynergyAbility")]
public class TriSynergyAbility : SynergyAbility, ITargetableAction
{
    public SynergyTagTrio[] SynergyTagTrios;
}
