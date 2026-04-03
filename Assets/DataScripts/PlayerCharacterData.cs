using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

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
    public CharacterBoard CharacterBoard;

    [Header("Resources")]
    public Sprite MenuImage;
    
    void OnEnable()
    {
        RefreshAllStats();
        if (CharacterBoard != null)
        {
            CharacterBoard.OnBoardChanged -= RefreshAbilityList;
            CharacterBoard.OnBoardChanged += RefreshAbilityList;
            CharacterBoard.OnBoardChanged -= RefreshStats;
            CharacterBoard.OnBoardChanged += RefreshStats;
        }
    }

    void OnDisable()
    {
        if (CharacterBoard != null)
        {
            CharacterBoard.OnBoardChanged -= RefreshAbilityList;
            CharacterBoard.OnBoardChanged -= RefreshStats;
        }
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
        if (level <= 1) return 0;         
        return Mathf.FloorToInt(100 * Mathf.Pow(level - 1, 1.5f)); 
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

    public void RefreshAbilityList()
    {
        abilities = abilities.Take(1).Concat(CharacterBoard.GetAllPlacedAbilities()).ToList();
    }

    public void RefreshStats()
    {
        RefreshAllStats();
        CharacterBoard.GetTotalStatBoosts().ToList().ForEach(pair => {
            if (pair.Key == Stat.MaxHp) MaxHp += pair.Value;
            else if (pair.Key == Stat.MaxMp) MaxMp += pair.Value;
            else if (pair.Key == Stat.Atk) Atk += pair.Value;
            else if (pair.Key == Stat.Mag) Mag += pair.Value;
            else if (pair.Key == Stat.Def) Def += pair.Value;
            else if (pair.Key == Stat.Mdef) Mdef += pair.Value;
            else if (pair.Key == Stat.Spd) Spd += pair.Value;
            else if (pair.Key == Stat.Acc) Acc += pair.Value;
            else if (pair.Key == Stat.Eva) Eva += pair.Value;
            else if (pair.Key == Stat.Luck) Luck += pair.Value;
        });
    }
}
