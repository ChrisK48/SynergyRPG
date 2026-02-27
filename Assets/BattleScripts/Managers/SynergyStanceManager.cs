using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SynergyStanceManager : MonoBehaviour
{
    public static SynergyStanceManager instance;
    private DualSynergyAbility[] masterSynergyList;

    public void Awake()
    {
        instance = this;
        masterSynergyList = Resources.LoadAll<DualSynergyAbility>("Synergies");
    }

    public void CreateSynergyStance(CharBattle[] users)
    {
        foreach (CharBattle user in users)
        {
            user.EnterSynergyStance();
        }
        BattleManager.instance.InsertSynergyStance(new SynergyStance(users));
        BattleManager.instance.NextTurn();
    }

    //TEMP FOR NOW PROBABLY NEEDS TO BE REWORKED LATER

    public List<DualSynergyAbility> GetAvailableSynergiesForStance(SynergyStance stance)
    {
        List<DualSynergyAbility> possibleSynergies = new List<DualSynergyAbility>();

        CharBattle charA = stance.users[0];
        CharBattle charB = stance.users[1];

        foreach (var synergy in masterSynergyList)
        {
            foreach (var recipe in synergy.synergyTagSets)
            {
                if (CanCharactersPerformRecipe(charA, charB, recipe))
                {
                    if (!possibleSynergies.Contains(synergy))
                    {
                        possibleSynergies.Add(synergy);
                    }
                }
            }
        }
        return possibleSynergies;
    }

    private bool CanCharactersPerformRecipe(CharBattle a, CharBattle b, SynergyTagSet recipe)
    {
        bool aHasTag1 = a.abilities.Any(abil => abil.SynergyTags.Contains(recipe.tag1));
        bool aHasTag2 = a.abilities.Any(abil => abil.SynergyTags.Contains(recipe.tag2));
        
        bool bHasTag1 = b.abilities.Any(abil => abil.SynergyTags.Contains(recipe.tag1));
        bool bHasTag2 = b.abilities.Any(abil => abil.SynergyTags.Contains(recipe.tag2));

        return (aHasTag1 && bHasTag2) || (aHasTag2 && bHasTag1);
    }
}
