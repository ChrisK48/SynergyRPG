using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBoard", menuName = "SkillSystem/Board")]
public class CharacterBoard : ScriptableObject
{
    public string characterName;
    public List<BoardTile> allTiles = new List<BoardTile>();
    public Vector2Int boardSize;
    public System.Action OnBoardChanged;

    public BoardTile GetTileAt(Vector2Int pos) => allTiles.Find(t => t.boardPosition == pos);

    public void SetTileAbility(Vector2Int pos, TileItem tile, Vector2Int origin)
    {
        BoardTile bTile = GetTileAt(pos);
        if (bTile != null)
        {
            bTile.placedTile = tile;
            bTile.originTile = origin;
            OnBoardChanged?.Invoke();
        }
    }

    // NEW: Clears the entire shape regardless of which segment you click
    public void RemoveTileAt(Vector2Int pos)
    {
        BoardTile targetTile = GetTileAt(pos);
        if (targetTile == null || targetTile.IsEmpty) return;

        TileItem tileToReturn = targetTile.placedTile;

        if (tileToReturn != null)
        {
            PartyManager.instance.SetTileEquipStatus(tileToReturn, false);
        }

        Vector2Int origin = targetTile.originTile;

        foreach (var tile in allTiles)
        {
            if (tile.originTile == origin)
            {
                tile.placedTile = null;
                tile.originTile = new Vector2Int(-1, -1);
            }
        }
        OnBoardChanged?.Invoke();
    }

    public List<Ability> GetAllPlacedAbilities()
    {
        List<Ability> abilities = new List<Ability>();
        foreach (BoardTile tile in allTiles)
        {
            if (!tile.IsEmpty && tile.placedTile != null && tile.placedTile.tileType is AbilityTile abilityTile && abilityTile.ability != null)
            {
                abilities.Add(abilityTile.ability);
            }
        }
        return abilities;
    }

    public Dictionary<Stat, int> GetTotalStatBoosts()
    {
        Dictionary<Stat, int> totals = new Dictionary<Stat, int>();

        foreach (BoardTile tile in allTiles)
        {
            if (!tile.IsEmpty && tile.originTile == tile.boardPosition)
            {
                if (tile.placedTile != null && tile.placedTile.tileType is StatTile statTile)
                {
                    if (!totals.ContainsKey(statTile.statType))
                    {
                        totals[statTile.statType] = 0;
                    }
                    
                    totals[statTile.statType] += statTile.statBoost;
                }
            }
        }

        return totals;
    }

    [ContextMenu("Reset All Tiles")]
    public void ResetAllTiles()
    {
        foreach (var tile in allTiles)
        {
            tile.originTile = new Vector2Int(-1, -1);
            tile.placedTile = null;
        }
    }
}