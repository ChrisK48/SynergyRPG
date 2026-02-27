using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;

[CreateAssetMenu(fileName = "New Two Person Synergy Ability", menuName = "Synergies/Two Person Synergy Ability")]
public class TwoMemberSynergyAbility : ScriptableObject, ITargetableAction
{
    public string synergyName;
    public string Name => synergyName;
    public string description;
    public TargetType targetType;
    public TargetType Targets => targetType;
    public List<SynergyTagSet> synergyTagSets = new List<SynergyTagSet>();
    [SerializeReference]
    public List<AbilityEffect> abilityEffects = new List<AbilityEffect>();

    public void PerformAction(CharBattle[] users, List<CharBattle> targets)
    {
        Debug.Log("Performing Two Person Synergy: " + Name);
        foreach (CharBattle user in users)
        {
            if (user is PlayerCharBattle player)
            {
                player.DeductPreppedAbilityResourceCosts();
            }
        }

        foreach (CharBattle target in targets)
        {
            ExecuteSynergy((PlayerCharBattle)users[0], (PlayerCharBattle)users[1], target);
        }

        foreach (CharBattle user in users)
        {
            user.EndPrep();
        }

        FlowManager.instance.GainFlow(20f); // This is also temporary until we have a better system for handling synergy resource costs and flow gain
    }

    public void ExecuteSynergy(PlayerCharBattle prepper, PlayerCharBattle activator, CharBattle target)
    {
        int calculatedPower = CalculatePower(prepper, activator);
        foreach (AbilityEffect effect in abilityEffects)
        {
            effect.ExecuteEffect(new CharBattle[] {prepper, activator}, target, calculatedPower);
        }
    }

    private int CalculatePower(PlayerCharBattle prepper, PlayerCharBattle activator)
    {
        int prepperstat = GetUserPower(prepper) * prepper.GetPreppedAbility().ScalingMultiplier;
        int activatorstat = GetUserPower(activator) * activator.GetPreppedAbility().ScalingMultiplier;
        float power = (prepperstat + activatorstat) * 1.5f; // Temp synergy power boost, can be adjusted or removed later
        return (int)power;
    }

    private int GetUserPower(CharBattle user)
    {
        switch (user.GetPreppedAbility().ScalingStat)
        {
            case ScalingStat.Atk:
                return user.Atk;
            case ScalingStat.Mag:
                return user.Mag;
            case ScalingStat.Def:
                return user.Def;
            case ScalingStat.Mdef:
                return user.Mdef;
            case ScalingStat.Spd:
                return user.Spd;
            case ScalingStat.Acc:
                return user.Acc;
            case ScalingStat.Eva:
                return user.Eva;
            case ScalingStat.Luck:
                return user.Luck;
            default:
                return 0;
        }
    }
}
