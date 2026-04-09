using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class GemType {}

[Serializable]
public class AbilityGem : GemType
{
    public Ability ability;
}

[Serializable]
public class StatGem : GemType
{
    public Stat statType;
    public int statBoost;
}

[Serializable]
public class BuffGem : GemType
{
    public Buff buff;
    public int buffDuration;
}


[CreateAssetMenu(fileName = "NewTile", menuName = "Item/Tile")]
public class GemItem : Item
{
    public List<Vector2Int> layoutOffsets = new List<Vector2Int>();
    [SerializeReference]
    public GemType GemType;
    public Sprite GemSprite;
    public Color GemColor;
    public bool IsExclusive;
}
