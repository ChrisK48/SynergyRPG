using System.Collections.Generic;
using UnityEngine;

public enum EquipSlot { Weapon, Armor, Accessory }

[CreateAssetMenu(fileName = "New Equippable", menuName = "Equippable")]
public class Equippable : Item
{
    public int MaxHpBonus, MaxMpBonus, AtkBonus, MagBonus, DefBonus, MdefBonus, SpdBonus, AccBonus, EvaBonus, LuckBonus;
    public EquipSlot equipSlot;
    public PlayerID charExclusive;
    [SerializeField] public List<GemSlot> gemSlots = new List<GemSlot>();
}

