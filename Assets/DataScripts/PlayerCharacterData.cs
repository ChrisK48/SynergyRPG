using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Player Character", menuName = "Player Character")]
public class PlayerCharData : ScriptableObject
{
    public string CharName;
    public int BaseMaxHp, BaseMaxMp, BaseAtk, BaseMag, BaseDef, BaseMdef, BaseSpd, BaseAcc, BaseEva, BaseLuck;
    public int currentHp, currentMp;
    public List<Ability> abilities;
    public int currentLevel;
    public int currentExp;
    public PlayerCharBattle charBattlePrefab;

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
