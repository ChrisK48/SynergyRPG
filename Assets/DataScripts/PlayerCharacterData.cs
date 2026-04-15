using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "New Player Character", menuName = "Player Character")]
public class PlayerCharData : ScriptableObject
{
    public string CharName;
    public int BaseMaxHp, BaseMaxMp, BaseAtk, BaseMag, BaseDef, BaseMdef, BaseSpd, BaseAcc, BaseEva, BaseLuck;
    [HideInInspector] public int MaxHp, MaxMp, Atk, Mag, Def, Mdef, Spd, Acc, Eva, Luck;
    public int currentHp, currentMp;
    public List<Ability> abilities;
    public EquipmentSlot weaponSlot = new EquipmentSlot { slotType = EquipSlot.Weapon };
    public EquipmentSlot armorSlot = new EquipmentSlot { slotType = EquipSlot.Armor };
    public EquipmentSlot accessorySlot = new EquipmentSlot { slotType = EquipSlot.Accessory };
    public int currentLevel;
    public int currentExp;
    public PlayerCharBattle charBattlePrefab;

    [Header("Resources")]
    public Sprite MenuImage;
    
    void OnEnable()
    {
        RefreshAllStats();
    }

    void OnDisable()
    {

    }

    public void RefreshAllStats()
    {
        InitializeStats();
        if (!weaponSlot.IsEmpty) ApplyEquipBonus(weaponSlot.currentItem);
        if (!armorSlot.IsEmpty) ApplyEquipBonus(armorSlot.currentItem);
        if (!accessorySlot.IsEmpty) ApplyEquipBonus(accessorySlot.currentItem);

        foreach (Gem gem in weaponSlot.equippedGems)
        {
            if (gem != null && gem.StatBonus.value != 0)
            {
                switch (gem.StatBonus.type)
                {
                    case Stat.MaxHp:
                        MaxHp += gem.StatBonus.value;
                        break;
                    case Stat.MaxMp:
                        MaxMp += gem.StatBonus.value;
                        break;
                    case Stat.Atk:
                        Atk += gem.StatBonus.value;
                        break;
                    case Stat.Mag:
                        Mag += gem.StatBonus.value;
                        break;
                    case Stat.Def:
                        Def += gem.StatBonus.value;
                        break;
                    case Stat.Mdef:
                        Mdef += gem.StatBonus.value;
                        break;
                    case Stat.Spd:
                        Spd += gem.StatBonus.value;
                        break;
                    case Stat.Acc:
                        Acc += gem.StatBonus.value;
                        break;
                    case Stat.Eva:
                        Eva += gem.StatBonus.value;
                        break;
                    case Stat.Luck:
                        Luck += gem.StatBonus.value;
                        break;
                }
            }
        }

        foreach (Gem gem in armorSlot.equippedGems)
        {
            if (gem != null && gem.StatBonus.value != 0)
            {
                switch (gem.StatBonus.type)
                {
                    case Stat.MaxHp:
                        MaxHp += gem.StatBonus.value;
                        break;
                    case Stat.MaxMp:
                        MaxMp += gem.StatBonus.value;
                        break;
                    case Stat.Atk:
                        Atk += gem.StatBonus.value;
                        break;
                    case Stat.Mag:
                        Mag += gem.StatBonus.value;
                        break;
                    case Stat.Def:
                        Def += gem.StatBonus.value;
                        break;
                    case Stat.Mdef:
                        Mdef += gem.StatBonus.value;
                        break;
                    case Stat.Spd:
                        Spd += gem.StatBonus.value;
                        break;
                    case Stat.Acc:
                        Acc += gem.StatBonus.value;
                        break;
                    case Stat.Eva:
                        Eva += gem.StatBonus.value;
                        break;
                    case Stat.Luck:
                        Luck += gem.StatBonus.value;
                        break;
                }
            }
        }

        foreach (Gem gem in accessorySlot.equippedGems)
        {
            if (gem != null && gem.StatBonus.value != 0)
            {
                switch (gem.StatBonus.type)
                {
                    case Stat.MaxHp:
                        MaxHp += gem.StatBonus.value;
                        break;
                    case Stat.MaxMp:
                        MaxMp += gem.StatBonus.value;
                        break;
                    case Stat.Atk:
                        Atk += gem.StatBonus.value;
                        break;
                    case Stat.Mag:
                        Mag += gem.StatBonus.value;
                        break;
                    case Stat.Def:
                        Def += gem.StatBonus.value;
                        break;
                    case Stat.Mdef:
                        Mdef += gem.StatBonus.value;
                        break;
                    case Stat.Spd:
                        Spd += gem.StatBonus.value;
                        break;
                    case Stat.Acc:
                        Acc += gem.StatBonus.value;
                        break;
                    case Stat.Eva:
                        Eva += gem.StatBonus.value;
                        break;
                    case Stat.Luck:
                        Luck += gem.StatBonus.value;
                        break;
                }
            }
        }

    }

    public void RefreshAbilities()
    {
        abilities.Clear();
        if (!weaponSlot.IsEmpty) abilities.AddRange(weaponSlot.equippedGems.FindAll(gem => gem != null && gem.GemAbility != null).ConvertAll(gem => gem.GemAbility));
        if (!armorSlot.IsEmpty) abilities.AddRange(armorSlot.equippedGems.FindAll(gem => gem != null && gem.GemAbility != null).ConvertAll(gem => gem.GemAbility));
        if (!accessorySlot.IsEmpty) abilities.AddRange(accessorySlot.equippedGems.FindAll(gem => gem != null && gem.GemAbility != null).ConvertAll(gem => gem.GemAbility));
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

    public EquipmentSlot GetEquipSlot(EquipSlot slotType)
    {
        switch (slotType)
        {
            case EquipSlot.Weapon:
                return weaponSlot;
            case EquipSlot.Armor:
                return armorSlot;
            case EquipSlot.Accessory:
                return accessorySlot;
            default:
                return null;
        }
    }
}
