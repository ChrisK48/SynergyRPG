using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBoard", menuName = "SkillSystem/Board")]
public class CharacterBoard : ScriptableObject
{
    public string characterName;
    public List<BoardTile> allTiles = new List<BoardTile>();
    public Vector2Int boardSize;



    public BoardTile GetTileAt(Vector2Int pos) => allTiles.Find(t => t.boardPosition == pos);
}