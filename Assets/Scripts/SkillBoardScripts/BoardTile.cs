using UnityEngine;


[System.Serializable]
public class BoardTile
{
    public Vector2Int boardPosition;
    public bool isUnlocked;    
    public TileItem placedTile; 
    
    [HideInInspector] public Vector2Int originTile = new Vector2Int(-1, -1);
    public bool IsEmpty => originTile.x == -1;
}
