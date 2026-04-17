using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;
using System.Linq;

[CreateAssetMenu(fileName = "New DualSynergy", menuName = "Synergies/DualSynergyAbility")]
public class DualSynergyAbility : SynergyAbility, ITargetableAction
{
    public List<SynergyTagSet> SynergyTagSets = new List<SynergyTagSet>();

    public SynergyTagSet GetValidTagSet(PlayerCharData charA, PlayerCharData charB, out bool isSwapped)
    {
        isSwapped = false;
        foreach (var tagSet in SynergyTagSets)
        {
            bool aHas1 = charA.abilities.Any(ab => ab.SynergyTags.Contains(tagSet.tag1));
            bool bHas2 = charB.abilities.Any(ab => ab.SynergyTags.Contains(tagSet.tag2));
            if (aHas1 && bHas2) return tagSet;

            bool aHas2 = charA.abilities.Any(ab => ab.SynergyTags.Contains(tagSet.tag2));
            bool bHas1 = charB.abilities.Any(ab => ab.SynergyTags.Contains(tagSet.tag1));
            if (aHas2 && bHas1) {
                isSwapped = true;
                return tagSet;
            }
        }
        return null;
    }

    public override Stat GetUserStat(CharBattle user)
    {
        List<SynergyTag> tags = user.GetStoredTags();
        foreach (var tagSet in SynergyTagSets)
        {
            if (tags.Contains(tagSet.tag1))
            {
                return tagSet.scalingStat1;
            } else if (tags.Contains(tagSet.tag2))
            {
                return tagSet.scalingStat2;
            }
        }
        throw new InvalidOperationException("No valid scaling stat found for the given synergy tags.");
    }

    public override int GetUserScalingMultiplier(CharBattle user)
    {
        List<SynergyTag> tags = user.GetStoredTags();
        foreach (var tagSet in SynergyTagSets)
        {
            if (tags.Contains(tagSet.tag1))
            {
                return tagSet.scalingMultiplier1;
            } else if (tags.Contains(tagSet.tag2))
            {
                return tagSet.scalingMultiplier2;
            }
        }
        throw new InvalidOperationException("No valid scaling multiplier found for the given synergy tags.");
    }

    public override int GetUserMpCost(CharBattle user)
    {
        List<SynergyTag> tags = user.GetStoredTags();
        foreach (var tagSet in SynergyTagSets)
        {
            if (tags.Contains(tagSet.tag1))
            {
                return tagSet.mpCost1;
            } else if (tags.Contains(tagSet.tag2))
            {
                return tagSet.mpCost2;
            }
        }
        throw new InvalidOperationException("No valid MP cost found for the given synergy tags.");
    }
}
