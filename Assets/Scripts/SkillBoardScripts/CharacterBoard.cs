using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBoard", menuName = "SkillSystem/Board")]
public class CharacterBoard : ScriptableObject
{
    [ContextMenu("Reset All Tiles")]
    public void ResetAllTiles()
    {
        foreach (var tile in allTiles)
        {
            tile.originTile = new Vector2Int(-1, -1);
            tile.placedTile = null;
        }
    }

    public string characterName;
    public List<BoardTile> allTiles = new List<BoardTile>();
    public Vector2Int boardSize;
    public System.Action OnBoardChanged;

    public BoardTile GetTileAt(Vector2Int pos) => allTiles.Find(t => t.boardPosition == pos);

    public void SetTileAbility(Vector2Int pos, Tile tile, Vector2Int origin)
    {
        BoardTile bTile = GetTileAt(pos);
        if (bTile != null)
        {
            bTile.placedTile = tile; // Stores the Tile SO
            bTile.originTile = origin;
            OnBoardChanged?.Invoke();
        }
    }

    public List<Ability> GetAllPlacedAbilities()
    {
        List<Ability> abilities = new List<Ability>();
        foreach (var tile in allTiles)
        {
            // Access the ability through the placedTile
            if (tile.placedTile != null)
            {
                abilities.Add(tile.placedTile.ability);
            }
        }
        return abilities;
    }
}