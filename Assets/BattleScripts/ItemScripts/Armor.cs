using UnityEngine;

public class Equipable : ScriptableObject, IItem
{
    public string EquipableName;
    public string EquipableDescription;
    public int MaxHp, MaxMp, Atk, Mag, Def, Mdef, Spd, Acc, Eva, Luck;
    public string Name => EquipableName;
    public string Description => EquipableDescription;
}
