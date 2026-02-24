using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stat Buff", menuName = "Buffs/Stat Buff")]
public class StatBuff : Buff
{
    public int AtkPercentChange, MatkPercentChange, DefPercentChange, MdefPercentChange, SpdPercentChange, LuckPercentChange, AccPercentChange, EvaPercentChange;

    public override void StartBuff(CharBattle target, ActiveBuff buffWrapper)
    {
        Debug.Log($"{target.charName} started Stat Buff: {buffName}");

        // Use a helper to apply and record each stat
        ApplyStatChange(target, buffWrapper, "Atk", AtkPercentChange, (val) => target.Atk += val);
        ApplyStatChange(target, buffWrapper, "Matk", MatkPercentChange, (val) => target.Matk += val);
        ApplyStatChange(target, buffWrapper, "Def", DefPercentChange, (val) => target.Def += val);
        ApplyStatChange(target, buffWrapper, "Mdef", MdefPercentChange, (val) => target.Mdef += val);
        ApplyStatChange(target, buffWrapper, "Spd", SpdPercentChange, (val) => target.Spd += val);
        ApplyStatChange(target, buffWrapper, "Acc", AccPercentChange, (val) => target.Acc += val);
        ApplyStatChange(target, buffWrapper, "Eva", EvaPercentChange, (val) => target.Eva += val);
        ApplyStatChange(target, buffWrapper, "Luck", LuckPercentChange, (val) => target.Luck += val);
    }

    private void ApplyStatChange(CharBattle target, ActiveBuff wrapper, string key, int percent, System.Action<int> applyEffect)
    {
        if (percent == 0) return;

        // Calculate the flat value based on current stat
        // For example: 50 Atk * 20 / 100 = 10
        int changeValue = 0;
        
        // Use a switch or reflection if you want to get the base value dynamically, 
        // but for now, we'll just pass the logic through the action.
        // We need the value to calculate the change:
        int currentVal = 0;
        switch(key) {
            case "Atk": currentVal = target.Atk; break;
            case "Matk": currentVal = target.Matk; break;
            case "Def": currentVal = target.Def; break;
            case "Mdef": currentVal = target.Mdef; break;
            case "Spd": currentVal = target.Spd; break;
            case "Acc": currentVal = target.Acc; break;
            case "Eva": currentVal = target.Eva; break;
            case "Luck": currentVal = target.Luck; break;
        }

        changeValue = currentVal * percent / 100;
        applyEffect(changeValue);
        
        // Store it so we can undo it later
        wrapper.statChanges[key] = changeValue;
    }

    public override void EndBuff(CharBattle target, ActiveBuff buffWrapper)
    {
        base.EndBuff(target, buffWrapper);

        // Undo whatever we stored in the dictionary
        if (buffWrapper.statChanges.TryGetValue("Atk", out int atk)) target.Atk -= atk;
        if (buffWrapper.statChanges.TryGetValue("Matk", out int matk)) target.Matk -= matk;
        if (buffWrapper.statChanges.TryGetValue("Def", out int def)) target.Def -= def;
        if (buffWrapper.statChanges.TryGetValue("Mdef", out int mdef)) target.Mdef -= mdef;
        if (buffWrapper.statChanges.TryGetValue("Spd", out int spd)) target.Spd -= spd;
        if (buffWrapper.statChanges.TryGetValue("Luck", out int luck)) target.Luck -= luck;
    }
}