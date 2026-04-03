using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;

public class BoardInteractionManager : MonoBehaviour
{
    public static BoardInteractionManager instance;
    public CharacterBoard activeBoard;
    
    public GameObject interactableTilePrefab;
    public Transform dragLayer;
    public Transform placedLayer;

    [HideInInspector] public List<SkillBoardTileUI> allTileUIs = new List<SkillBoardTileUI>();

    void Awake() => instance = this;

    // Helper to find the physical tile at a coordinate
    public SkillBoardTileUI GetTileUIAt(Vector2Int pos)
    {
        return allTileUIs.Find(t => t.boardPosition == pos);
    }

    public void RebuildBoardUI(CharacterBoard board)
    {
        StartCoroutine(RebuildRoutine(board));
    }

    private IEnumerator RebuildRoutine(CharacterBoard board)
    {
        activeBoard = board;
        ClearExistingStickers();

        // WAIT for one frame. 
        // This allows the GridLayoutGroup to position the SkillBoardTileUIs.
        yield return new WaitForEndOfFrame();

        foreach (var tileData in activeBoard.allTiles)
        {
            if (tileData.placedTile != null)
            {
                SpawnStickerFromData(tileData);
            }
        }
    }

    private void SpawnStickerFromData(BoardTile data)
    {
        SkillBoardTileUI tileUI = GetTileUIAt(data.boardPosition);

        if (tileUI != null && data.placedTile != null)
        {
            GameObject sticker = Instantiate(interactableTilePrefab, placedLayer);
            InteractableTile script = sticker.GetComponent<InteractableTile>();
            
            script.Setup(data.placedTile);
            script.isSlotted = true; 
            
            // We set the WORLD position to match the physical tile's WORLD position
            sticker.transform.position = tileUI.transform.position;
            sticker.transform.SetAsLastSibling();
        }
    }

    private void ClearExistingStickers()
    {
        InteractableTile[] existing = FindObjectsOfType<InteractableTile>();
        foreach (var s in existing)
        {
            if (s.isSlotted) Destroy(s.gameObject);
        }
    }


    public void SpawnSticker(TileItem tileData)
    {
        GameObject sticker = Instantiate(interactableTilePrefab, dragLayer);
        InteractableTile script = sticker.GetComponent<InteractableTile>();
        
        script.Setup(tileData);

        // 2. Change this line right here:
        sticker.transform.position = Mouse.current.position.ReadValue();
    }

    public SkillBoardTileUI GetSlotAt(Vector2Int coords)
    {
        // We search the UI Grid for the slot that matches these coordinates
        // Assuming your slots are children of a grid object
        SkillBoardTileUI[] allSlots = GetComponentsInChildren<SkillBoardTileUI>();
        foreach (var slot in allSlots)
        {
            if (slot.boardPosition == coords) return slot;
        }
        return null;
    }
}