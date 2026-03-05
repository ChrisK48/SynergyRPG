using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class SynergySearchLogic
{
    private DualSynergyAbility[] masterSynergyList;
    private TriSynergyAbility[] masterTriSynergyList;

    public SynergySearchLogic()
    {
        masterSynergyList = Resources.LoadAll<DualSynergyAbility>("Synergies/Dual");
        masterTriSynergyList = Resources.LoadAll<TriSynergyAbility>("Synergies/Tri");
    }

    public List<Tuple<DualSynergyAbility, CharBattle>> GetDoubleSynergy(CharBattle currentActor, Ability currentAbility)
    {
        List<Tuple<DualSynergyAbility, CharBattle>> matchingSynergies = new List<Tuple<DualSynergyAbility, CharBattle>>();

        foreach (var potentialPartner in BattleManager.instance.playerChars.Where(pc => pc.GetIfAlive() && pc != currentActor))
        {
            Ability prepped = potentialPartner.GetPreppedAbility();
            
            if (prepped != null && potentialPartner != currentActor)
            {
                foreach (var synergy in masterSynergyList)
                {
                    foreach (var recipe in synergy.SynergyTagSets)
                    {
                        if (CheckDuoMatch(currentAbility, prepped, recipe))
                        {
                            matchingSynergies.Add(new Tuple<DualSynergyAbility, CharBattle>(synergy, potentialPartner));
                        }
                    }
                }
            }
        }
        return matchingSynergies;
    }

    public List<Tuple<TriSynergyAbility, CharBattle[]>> GetTripleSynergy(CharBattle currentActor, Ability currentAbility)
    {
        List<Tuple<TriSynergyAbility, CharBattle[]>> matchingSynergies = new List<Tuple<TriSynergyAbility, CharBattle[]>>();

        foreach (var stance in BattleManager.instance.GetSynergyStances())
        {
                CharBattle potentialPartner1 = stance.users[0];
                CharBattle potentialPartner2 = stance.users[1];
                Ability prepped1 = potentialPartner1.GetPreppedAbility();
                Ability prepped2 = potentialPartner2.GetPreppedAbility();

                if (prepped1 != null && prepped2 != null && potentialPartner1 != currentActor && potentialPartner2 != currentActor && potentialPartner1 != potentialPartner2)
                {
                    foreach (var synergy in masterTriSynergyList)
                    {
                        foreach (var recipe in synergy.SynergyTagTrios)
                        {
                            if (CheckTrioMatch(currentAbility, prepped1, prepped2, recipe))
                            {
                                matchingSynergies.Add(new Tuple<TriSynergyAbility, CharBattle[]>(synergy, new CharBattle[] { potentialPartner1, potentialPartner2 }));
                            }
                        }
                    }
                }
        }
        
        return matchingSynergies;
    }


    private bool CheckDuoMatch(Ability a, Ability b, SynergyTagSet recipe)
    {
        bool aMatches1 = a.SynergyTags.Contains(recipe.tag1);
        bool aMatches2 = a.SynergyTags.Contains(recipe.tag2);
        bool bMatches1 = b.SynergyTags.Contains(recipe.tag1);
        bool bMatches2 = b.SynergyTags.Contains(recipe.tag2);

        return (aMatches1 && bMatches2) || (aMatches2 && bMatches1);
    }

    private bool CheckTrioMatch(Ability a, Ability b, Ability c, SynergyTagTrio recipe)
    {
        bool aHasTag1 = a.SynergyTags.Contains(recipe.tag1);
        bool aHasTag2 = a.SynergyTags.Contains(recipe.tag2);
        bool aHasTag3 = a.SynergyTags.Contains(recipe.tag3);

        bool bHasTag1 = b.SynergyTags.Contains(recipe.tag1);
        bool bHasTag2 = b.SynergyTags.Contains(recipe.tag2);
        bool bHasTag3 = b.SynergyTags.Contains(recipe.tag3);

        bool cHasTag1 = c.SynergyTags.Contains(recipe.tag1);
        bool cHasTag2 = c.SynergyTags.Contains(recipe.tag2);
        bool cHasTag3 = c.SynergyTags.Contains(recipe.tag3);

        if (aHasTag1 && bHasTag2 && cHasTag3 || 
            aHasTag1 && bHasTag3 && cHasTag2 || 
            aHasTag2 && bHasTag1 && cHasTag3 || 
            aHasTag2 && bHasTag3 && cHasTag1 || 
            aHasTag3 && bHasTag1 && cHasTag2 || 
            aHasTag3 && bHasTag2 && cHasTag1)
        {
            return true;
        }
        
        return false;
    }

    public List<Tuple<DualSynergyAbility, CharBattle>> GetPotentialPairings(CharBattle currentActor, Ability currentAbility)
    {
        List<Tuple<DualSynergyAbility, CharBattle>> matchingSynergies = new List<Tuple<DualSynergyAbility, CharBattle>>();

        foreach (PlayerCharBattle player in BattleManager.instance.playerEntities)
        {
            foreach(var ability in player.abilities)
            {
                foreach (var synergy in masterSynergyList)
                {
                    foreach (var recipe in synergy.SynergyTagSets)
                    {
                        if (CheckDuoMatch(currentAbility, ability, recipe))
                        {
                            matchingSynergies.Add(new Tuple<DualSynergyAbility, CharBattle>(synergy, player));
                        }
                    }
                }   
            }
        }
        return matchingSynergies;
    }

    public List<Tuple<TriSynergyAbility, CharBattle[]>> GetPotentialTriPairings(CharBattle currentActor, Ability currentAbility)
    {
        List<Tuple<TriSynergyAbility, CharBattle[]>> matchingSynergies = new List<Tuple<TriSynergyAbility, CharBattle[]>>();

        foreach (var stance in BattleManager.instance.GetSynergyStances())
        {
                CharBattle potentialPartner1 = stance.users[0];
                CharBattle potentialPartner2 = stance.users[1];
                
                foreach(var ability1 in potentialPartner1.abilities)
                {
                    foreach(var ability2 in potentialPartner2.abilities)
                    {
                        foreach (var synergy in masterTriSynergyList)
                        {
                            foreach (var recipe in synergy.SynergyTagTrios)
                            {
                                if (CheckTrioMatch(currentAbility, ability1, ability2, recipe))
                                {
                                    matchingSynergies.Add(new Tuple<TriSynergyAbility, CharBattle[]>(synergy, new CharBattle[] { potentialPartner1, potentialPartner2 }));
                                }
                            }
                        }
                    }
                }
        }
        
        return matchingSynergies;
    }
}
