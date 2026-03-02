using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class SynergyStanceManager : MonoBehaviour
{
    public static SynergyStanceManager instance;
    private DualSynergyAbility[] masterSynergyList;
    private bool stanceExists;

    public void Awake()
    {
        instance = this;
        masterSynergyList = Resources.LoadAll<DualSynergyAbility>("Synergies");
    }

    public void CreateSynergyStance(CharBattle[] users)
    {
        SynergyStance newStance = new SynergyStance(users);
        foreach (CharBattle user in users)
        {
            user.EnterSynergyStance(newStance);
        }
        stanceExists = true;
        BattleManager.instance.InsertSynergyStance(newStance);
        BattleManager.instance.NextTurn();
    }

    public void BreakSynergyStance(SynergyStance stance)
    {
        foreach (CharBattle user in stance.users)
        {
            user.ExitSynergyStance();
        }
        stanceExists = false;
        BattleManager.instance.playerEntities.AddRange(stance.users);
        BattleManager.instance.playerEntities.Remove(stance);
        BattleManager.instance.GetSynergyStances().Remove(stance);
        BattleUIManager.instance.UpdateTurnOrderUI(BattleManager.instance.GetTurnManager().GetTurnOrder(), BattleManager.instance.GetTurnManager().getCurrentChar(), BattleManager.instance.GetTurnManager().GetCurrentTurnIndex());
        if (stance.users[0] is PlayerCharBattle)FlowManager.instance.LoseFlow(30); // For now lose 30 flow on synergy break. Will probably adjust later.
    }

    public bool GetIfStanceExists() { return stanceExists; }

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
