using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stat Buff", menuName = "Buffs/Stat Buff")]
public class StatBuff : Buff
{
    public int AtkPercentChange;
    public int MatkPercentChange;
    public int DefPercentChange;
    public int MdefPercentChange;
    public int SpdPercentChange;
    public int LuckPercentChange;
    private int originalAtk;
    private int originalMatk;
    private int originalDef;
    private int orginalMdef;
    private int orginalSpd;
    private int orginalLuck;
    public override void StartBuff(CharBattle target)
    {
        Debug.Log(target.charName + " has started Stat Buff.");
        if (AtkPercentChange != 0)
        {
            originalAtk = target.Atk;
            target.Atk += target.Atk * AtkPercentChange / 100;
        }
        if (MatkPercentChange != 0)
        {
            originalMatk = target.Matk;
            target.Matk += target.Matk * MatkPercentChange / 100;
        }
        if (DefPercentChange != 0)
        {
            originalDef = target.Def;
            target.Def += target.Def * DefPercentChange / 100;
        }
        if (MdefPercentChange != 0)
        {
            orginalMdef = target.Mdef;
            target.Mdef += target.Mdef * MdefPercentChange / 100;
        }
        if (SpdPercentChange != 0)
        {
            orginalSpd = target.Spd;
            target.Spd += target.Spd * SpdPercentChange / 100;
        }
        if (LuckPercentChange != 0)
        {
            orginalLuck = target.Luck;
            target.Luck += target.Luck * LuckPercentChange / 100;
        }
    }

    public override void EndBuff(CharBattle target)
    {
        base.EndBuff(target);
        if (AtkPercentChange != 0)
        {
            target.Atk = originalAtk;
        }
        if (MatkPercentChange != 0)
        {
            target.Matk = originalMatk;
        }
        if (DefPercentChange != 0)
        {
            target.Def = originalDef;
        }
        if (MdefPercentChange != 0)
        {
            target.Mdef = orginalMdef;
        }
        if (SpdPercentChange != 0)
        {
            target.Spd = orginalSpd;
        }
        if (LuckPercentChange != 0)
        {
            target.Luck = orginalLuck;
        }
        target.activeBuffs.Remove(this);
    }
}
