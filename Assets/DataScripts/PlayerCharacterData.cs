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
}
