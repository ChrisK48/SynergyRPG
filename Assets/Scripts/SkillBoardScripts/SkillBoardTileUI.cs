using UnityEngine;
using UnityEngine.UI;

public class SkillBoardTileUI : MonoBehaviour
{
    public Image background;
    public Vector2Int boardPosition;
    public BoardTile currentTile;
    public void Initialize(BoardTile tile)
    {
        if (tile is null)
        {
            background.enabled = false;
            return;
        }
        else if (!tile.isUnlocked && tile.placedAbility == null)
        {
            background.color = Color.gray;
        }
        else
        {
            background.color = Color.white; // Or your tile sprite
        }

        currentTile = tile;
        boardPosition = tile.boardPosition;
    }
}
