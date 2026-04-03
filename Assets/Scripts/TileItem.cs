using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class TileType {}

[Serializable]
public class AbilityTile : TileType
{
    public Ability ability;
}

[Serializable]
public class StatTile : TileType
{
    public Stat statType;
    public int statBoost;
}

[Serializable]
public class BuffTile : TileType
{
    public Buff buff;
    public int buffDuration;
}

[Serializable]
public struct TileOffset
{
    public int x;
    public int y;
}

[CreateAssetMenu(fileName = "NewTile", menuName = "Item/Tile")]
public class TileItem : Item
{
    public List<TileOffset> layoutOffsets = new List<TileOffset>();
    [SerializeReference]
    public TileType tileType;
    public Sprite TileSprite;
    public Color tileColor;
    public bool isExclusive;
}

[Serializable]
public class TileInventoryEntry
{
    public TileItem tile;
    [HideInInspector] public bool isEquipped;

    public TileInventoryEntry(TileItem tile)
    {
        this.tile = tile;
        isEquipped = false;
    }
}
