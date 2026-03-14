using UnityEngine;

public enum EquipSlot { Weapon, Head, Body, Accessory }

[CreateAssetMenu(fileName = "New Equippable", menuName = "Equippable")]
public class Equippable : Item
{
    public int MaxHpBonus, MaxMpBonus, AtkBonus, MagBonus, DefBonus, MdefBonus, SpdBonus, AccBonus, EvaBonus, LuckBonus;
    public EquipSlot equipSlot;
}
