using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;
public struct TripleMatchData
{
    public SynergyTagTrio matchedSet;
    public SynergyTag userTagA, userTagB, userTagC;
}

[CreateAssetMenu(fileName = "New TriSynergy", menuName = "Synergies/TriSynergyAbility")]
public class TriSynergyAbility : SynergyAbility, ITargetableAction
{
    public SynergyTagTrio[] SynergyTagTrios;
    public TripleMatchData? GetValidTripleSet(PlayerCharData a, PlayerCharData b, PlayerCharData c)
    {
        foreach (var trio in SynergyTagTrios)
        {
            PlayerCharData[] players = { a, b, c };
            
            // We need to find if there is a 1-to-1 mapping for Tag1, Tag2, and Tag3
            // among the three players provided.
            int[][] permutations = new int[][] {
                new[] {0, 1, 2}, new[] {0, 2, 1},
                new[] {1, 0, 2}, new[] {1, 2, 0},
                new[] {2, 0, 1}, new[] {2, 1, 0}
            };

            foreach (var p in permutations)
            {
                // Use the EXACT logic from your DualSynergy: .abilities.Any -> .SynergyTags.Contains
                bool has1 = players[p[0]].abilities.Any(ab => ab.SynergyTags.Contains(trio.tag1));
                bool has2 = players[p[1]].abilities.Any(ab => ab.SynergyTags.Contains(trio.tag2));
                bool has3 = players[p[2]].abilities.Any(ab => ab.SynergyTags.Contains(trio.tag3));

                if (has1 && has2 && has3)
                {
                    return new TripleMatchData {
                        matchedSet = trio,
                        userTagA = p[0] == 0 ? trio.tag1 : (p[1] == 0 ? trio.tag2 : trio.tag3),
                        userTagB = p[0] == 1 ? trio.tag1 : (p[1] == 1 ? trio.tag2 : trio.tag3),
                        userTagC = p[0] == 2 ? trio.tag1 : (p[1] == 2 ? trio.tag2 : trio.tag3)
                    };
                }
            }
        }
        return null;
    }

    private bool HasTag(PlayerCharData p, SynergyTag t) => p.abilities.Any(ab => ab.SynergyTags.Contains(t));

    public override Stat GetUserStat(CharBattle user)
    {
        List<SynergyTag> tags = user.GetStoredTags();
        foreach (SynergyTagTrio trio in SynergyTagTrios)
        {
            if (tags.Contains(trio.tag1)) return trio.scalingStat1;
            else if (tags.Contains(trio.tag2)) return trio.scalingStat2;
            else if (tags.Contains(trio.tag3)) return trio.scalingStat3;
        }
        throw new InvalidOperationException("No valid scaling multiplier found for the given synergy tags.");
    }

    public override int GetUserScalingMultiplier(CharBattle user)
    {
        List<SynergyTag> tags = user.GetStoredTags();
        foreach (SynergyTagTrio trio in SynergyTagTrios)
        {
            if (tags.Contains(trio.tag1)) return trio.scalingMultiplier1;
            else if (tags.Contains(trio.tag2)) return trio.scalingMultiplier2;
            else if (tags.Contains(trio.tag3)) return trio.scalingMultiplier3;
        }
        throw new InvalidOperationException("No valid scaling multiplier found for the given synergy tags.");
    }
    
    public override int GetUserMpCost(CharBattle user)
    {
        List<SynergyTag> tags = user.GetStoredTags();
        foreach (SynergyTagTrio trio in SynergyTagTrios)
        {
            if (tags.Contains(trio.tag1)) return trio.mpCost1;
            else if (tags.Contains(trio.tag2)) return trio.mpCost2;
            else if (tags.Contains(trio.tag3)) return trio.mpCost3;
        }
        throw new InvalidOperationException("No valid mp cost found for the given synergy tags.");
    }
}

