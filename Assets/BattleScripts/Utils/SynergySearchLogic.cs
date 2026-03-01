using UnityEngine;

public class SynergySearchLogic
{
    private DualSynergyAbility[] masterSynergyList;
    private TriSynergyAbility[] masterTriSynergyList;

    public SynergySearchLogic()
    {
        masterSynergyList = Resources.LoadAll<DualSynergyAbility>("Synergies/Dual");
        masterTriSynergyList = Resources.LoadAll<TriSynergyAbility>("Synergies/Tri");
    }

    public DualSynergyAbility GetDoubleSynergy(CharBattle currentActor, Ability currentAbility, out CharBattle partner)
    {
        partner = null;

        foreach (var potentialPartner in BattleManager.instance.playerChars)
        {
            if (potentialPartner.GetIfInSynergyStance()) continue;
            Ability prepped = potentialPartner.GetPreppedAbility();
            
            if (prepped != null && potentialPartner != currentActor)
            {
                foreach (var synergy in masterSynergyList)
                {
                    foreach (var recipe in synergy.synergyTagSets)
                    {
                        if (CheckDuoMatch(currentAbility, prepped, recipe))
                        {
                            partner = potentialPartner;
                            return synergy;
                        }
                    }
                }
            }
        }
        return null;
    }

    public TriSynergyAbility GetTripleSynergy(Ability a, Ability b, Ability c)
    {
        if (c == null) return null;
        foreach (var triSynergyAbility in masterTriSynergyList)
        {
            foreach (var recipe in triSynergyAbility.SynergyTagTrios)
            {
                if (CheckTrioMatch(a, b, c, recipe))
                {
                    return triSynergyAbility;
                }
            }
        }
        return null;
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
}
