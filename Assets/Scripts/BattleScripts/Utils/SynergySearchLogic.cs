using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using JetBrains.Annotations;

public struct DualSynergyResult
{
    public DualSynergyAbility Ability;
    public PlayerCharData UserA;
    public SynergyTag UserATag;
    public PlayerCharData UserB;
    public SynergyTag UserBTag;

    public DualSynergyResult(DualSynergyAbility ability, PlayerCharData userA, SynergyTag userATag, PlayerCharData userB, SynergyTag userBTag)
    {
        Ability = ability;
        UserA = userA;
        UserATag = userATag;
        UserB = userB;
        UserBTag = userBTag;
    }
}

public struct TriSynergyResult
{
    public TriSynergyAbility Ability;
    public PlayerCharData UserA;
    public PlayerCharData UserB;
    public PlayerCharData UserC;
    public SynergyTag UserATag;
    public SynergyTag UserBTag;
    public SynergyTag UserCTag;

    public TriSynergyResult(TriSynergyAbility ab, PlayerCharData a, PlayerCharData b, PlayerCharData c, SynergyTag tagA, SynergyTag tagB, SynergyTag tagC)
    {
        Ability = ab;
        UserA = a;
        UserB = b;
        UserC = c;
        UserATag = tagA;
        UserBTag = tagB;
        UserCTag = tagC;
    }
}

public class SynergySearchLogic
{
    private DualSynergyAbility[] masterSynergyList;
    private TriSynergyAbility[] masterTriSynergyList;

    public SynergySearchLogic()
    {
        masterSynergyList = Resources.LoadAll<DualSynergyAbility>("Synergies/Dual");
        masterTriSynergyList = Resources.LoadAll<TriSynergyAbility>("Synergies/Tri");
    }

    public List<DualSynergyResult> GetDoubleSynergies(List<PlayerCharData> activeMembers)
    {
        var results = new List<DualSynergyResult>();

        for (int i = 0; i < activeMembers.Count; i++)
        {
            for (int j = i + 1; j < activeMembers.Count; j++)
            {
                var charA = activeMembers[i];
                var charB = activeMembers[j];

                foreach (var synergy in masterSynergyList)
                {
                    var validSet = synergy.GetValidTagSet(charA, charB, out bool swapped);
                    
                    if (validSet != null) 
                    {
                        // If swapped is FALSE: CharA has Tag1, CharB has Tag2
                        // If swapped is TRUE:  CharA has Tag2, CharB has Tag1
                        SynergyTag tagForA = !swapped ? validSet.tag1 : validSet.tag2;
                        SynergyTag tagForB = !swapped ? validSet.tag2 : validSet.tag1;

                        results.Add(new DualSynergyResult(synergy, charA, tagForA, charB, tagForB));
                    }
                }
            }
        }
        return results;
    }

    public List<TriSynergyResult> GetTripleSynergies(List<PlayerCharData> activeMembers)
    {
        var results = new List<TriSynergyResult>();

        for (int i = 0; i < activeMembers.Count; i++)
        {
            for (int j = i + 1; j < activeMembers.Count; j++)
            {
                for (int k = j + 1; k < activeMembers.Count; k++)
                {
                    var charA = activeMembers[i];
                    var charB = activeMembers[j];
                    var charC = activeMembers[k];

                    foreach (var synergy in masterTriSynergyList)
                    {
                        var match = synergy.GetValidTripleSet(charA, charB, charC);
                        if (match.HasValue)
                        {
                            results.Add(new TriSynergyResult(
                                synergy, charA, charB, charC, match.Value.userTagA, match.Value.userTagB, match.Value.userTagC
                            ));
                        }
                    }
                }
            }
        }
        return results;
    }
}
