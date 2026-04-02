using UnityEngine;

public enum AspectType
{
    None,
    Power,
    Flow,
    Sp
}

[System.Serializable]
public class BoardTile
{
    public Vector2Int boardPosition;
    public bool isUnlocked;
    public AspectType aspectType;
    
    // Change this from Ability to Tile
    public Tile placedTile; 
    
    [HideInInspector] public Vector2Int originTile = new Vector2Int(-1, -1);
    public bool IsEmpty => originTile.x == -1;
}
