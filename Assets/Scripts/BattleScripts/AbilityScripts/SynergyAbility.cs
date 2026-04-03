using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public abstract class SynergyAbility : ScriptableObject, ITargetableAction
{
    public string synergyName;
    public string Name => synergyName;
    public string description;
    public float synergyPowerMultiplier;
    public TargetType targetType;
    public TargetType Targets => targetType;

    [SerializeReference]
    public List<AbilityEffect> abilityEffects = new List<AbilityEffect>();

    public void PerformAction(CharBattle[] users, List<ITurnEntity> targets, System.Action onComplete = null)
    {
        foreach (CharBattle user in users)
        {
            if (user is PlayerCharBattle player)
            {
                player.DeductPreppedAbilityResourceCosts();
            }
        }

        foreach (CharBattle target in targets)
        {
            ExecuteSynergy(users, target);
        }

        foreach (CharBattle user in users)
        {
            user.EndPrep();
            if (user.GetIfInSynergyStance())
            {
                SynergyStance stance = user.GetCurrentSynergyStance();
                stance.EndPrep();
            }
        }

        if (!users.Any(u => u.GetIfInSynergyStance()) && !BattleUIManager.instance.GetIfFastTracked()) FlowManager.instance.GainFlow(10); // This is also temporary until we have a better system for handling synergy resource costs and flow gain
        onComplete?.Invoke();
    }

    public void ExecuteSynergy(CharBattle[] users, CharBattle target)
    {
        int calculatedPower = CalculatePower(users);
        foreach (AbilityEffect effect in abilityEffects)
        {
            effect.ExecuteEffect(users, target, calculatedPower);
        }
    }

    protected int CalculatePower(CharBattle[] users)
    {
        float power = 0f;
        foreach (CharBattle user in users)
        {
            power += GetUserPower(user) * user.GetPreppedAbility().ScalingMultiplier;
        }
        power *= synergyPowerMultiplier; // Use the synergy power multiplier
        return (int)power;
    }

    protected int GetUserPower(CharBattle user)
    {
        switch (user.GetPreppedAbility().ScalingStat)
        {
            case Stat.Atk:
                return user.Atk;
            case Stat.Mag:
                return user.Mag;
            case Stat.Def:
                return user.Def;
            case Stat.Mdef:
                return user.Mdef;
            case Stat.Spd:
                return user.Spd;
            case Stat.Acc:
                return user.Acc;
            case Stat.Eva:
                return user.Eva;
            case Stat.Luck:
                return user.Luck;
            default:
                return 0;
        }
    }
}
