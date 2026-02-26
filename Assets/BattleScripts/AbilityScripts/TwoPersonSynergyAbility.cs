using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Two Person Synergy Ability", menuName = "Synergies/Two Person Synergy Ability")]
public class TwoMemberSynergyAbility : ScriptableObject, ITargetableAction
{
    public string synergyName;
    public string Name => synergyName;
    public string description;
    public TargetType targetType;
    public TargetType Targets => targetType;
    public Ability requiredAbility1;
    public Ability requiredAbility2;
    [SerializeReference]
    public List<AbilityEffect> abilityEffects = new List<AbilityEffect>();

    public void PerformAction(CharBattle[] users, List<CharBattle> targets)
    {
        Debug.Log("Performing Two Person Synergy: " + Name);
        foreach (CharBattle user in users)
        {
            if (user is PlayerCharBattle player)
            {
                GetAbilityPart(player);
                player.DeductSynergyResourceCosts();
            }
        }

        foreach (CharBattle target in targets)
        {
            ExecuteSynergy((PlayerCharBattle)users[0], (PlayerCharBattle)users[1], target);
        }
    }

    public void ExecuteSynergy(PlayerCharBattle prepper, PlayerCharBattle activator, CharBattle target)
    {
        foreach (AbilityEffect effect in abilityEffects)
        {
            effect.ExecuteEffect(new CharBattle[] {prepper, activator}, target);
        }
    }

    public bool IsPossible(CharBattle activeChar, List<PlayerCharBattle> party)
    {
        if (!activeChar.isAlive) return false;

        Ability missingAbility = activeChar.abilities.Contains(requiredAbility1) ? 
                                requiredAbility2 : requiredAbility1;

        foreach (var member in party)
        {
            if (member == activeChar) continue;

            if (member.isAlive && member.abilities.Contains(missingAbility)) 
            {
                return true;
            }
        }

        return false;
    }

    public void Prep(CharBattle prepper)
    {
        prepper.isPreppingSynergy = true;
        prepper.currentSynergy = this;
        BattleManager.instance.NextTurn();
    }

    // This method is 1000% temporary until we have a better system for handling synergy "parts" and resource costs
    public void GetAbilityPart(PlayerCharBattle player)
    {
        if (player.abilities.Contains(requiredAbility1))
        {
            player.StoreSynergy(requiredAbility1);
        }
        else
        {
            player.StoreSynergy(requiredAbility2);
        }
    }
}
