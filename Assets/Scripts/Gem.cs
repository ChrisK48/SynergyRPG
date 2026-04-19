using System;
using UnityEngine;

public enum GemType {CharacterAbility, GeneralAbility, Passive}

[Serializable]
public struct StatBonusEntry
{
    public Stat type;
    public int value;
}

[CreateAssetMenu(fileName = "New Gem", menuName = "Item/Gem")]
public class Gem : Item
{
    public Sprite GemSprite;
    public Color GemColor;
    public GemType GemType;
    public Ability GemAbility;
    public StatBonusEntry StatBonus;
    public PlayerID charExclusive;
    void OnValidate() => itemType = ItemType.Gem;
}
