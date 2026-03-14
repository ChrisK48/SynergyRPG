using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Player Character", menuName = "Player Character")]
public class PlayerCharData : ScriptableObject
{
    public string CharName;
    public int BaseMaxHp, BaseMaxMp, BaseAtk, BaseMag, BaseDef, BaseMdef, BaseSpd, BaseAcc, BaseEva, BaseLuck;
    [HideInInspector]
    public int MaxHp, MaxMp, Atk, Mag, Def, Mdef, Spd, Acc, Eva, Luck;
    public int currentHp, currentMp;
    public List<Ability> abilities;
    public Equippable weapon, head, body, accessory1, accessory2;
    public int currentLevel;
    public int currentExp;
    public PlayerCharBattle charBattlePrefab;

    void OnEnable()
    {
        RefreshAllStats();
    }

    public void RefreshAllStats()
    {
        InitializeStats();
        if (weapon != null) ApplyEquipBonus(weapon);
        if (head != null) ApplyEquipBonus(head);
        if (body != null) ApplyEquipBonus(body);
        if (accessory1 != null) ApplyEquipBonus(accessory1);
        if (accessory2 != null) ApplyEquipBonus(accessory2);
    }

    public void InitializeStats()
    {
        MaxHp = BaseMaxHp;
        MaxMp = BaseMaxMp;
        Atk = BaseAtk;
        Mag = BaseMag;
        Def = BaseDef;
        Mdef = BaseMdef;
        Spd = BaseSpd;
        Acc = BaseAcc;
        Eva = BaseEva;
        Luck = BaseLuck;
    }

    public void ApplyEquipBonus(Equippable equip)
    {
        MaxHp += equip.MaxHpBonus;
        MaxMp += equip.MaxMpBonus;
        Atk += equip.AtkBonus;
        Mag += equip.MagBonus;
        Def += equip.DefBonus;
        Mdef += equip.MdefBonus;
        Spd += equip.SpdBonus;
        Acc += equip.AccBonus;
        Eva += equip.EvaBonus;
        Luck += equip.LuckBonus;
    }

    public int GetExpToNextLevel(int level)
    {
        if (level <= 1) return 100;
        return Mathf.FloorToInt(100 * Mathf.Pow(level - 1, 1.5f)); // May change exp curve later (probably)
    }

    public float GainExp(int expAmount)
    {
        currentExp += expAmount;
        while (currentExp >= GetExpToNextLevel(currentLevel+1))
        {
            LevelUp();
        }
        return 0;
    }

    public int GetLevelForXP(int xp)
    {
        int level = 1;
        while (xp >= GetExpToNextLevel(level + 1))
        {
            level++;
        }
        return level;
    }

    void LevelUp()
    {
        currentLevel++;
        Debug.Log(CharName + " leveled up to level " + currentLevel + "!");
        // Will add stat curve stuff here
    }
}
