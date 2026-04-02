using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEditor; // Add this at the top!

public class InteractableTile : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public Tile sourceTile;
    public Ability abilityData;
    private Image displayImage;
    private RectTransform rect;
    public bool isSlotted = false;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        displayImage = GetComponent<Image>();
    }

    void Update()
    {
        if (isSlotted) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        RectTransform boardRect = MenuManager.instance.SkillBoardContainer.GetComponentInChildren<GridLayoutGroup>().GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(boardRect, mousePos, null, out Vector2 localPoint);

        // X: Floor to find the left edge of the column
        float snapX = Mathf.Floor(localPoint.x / 50f) * 50f;
        
        // Y: Since localPoint.y is negative, we use Ceil or Floor to find the top edge.
        // If it's jumping one square too high/low, swap Ceil for Floor.
        float snapY = Mathf.Ceil(localPoint.y / 50f) * 50f;

        // Convert back to world space. 
        // Notice: We removed the "-25f" because the Pivot (0,1) matches the snap point (0,0) perfectly now.
        transform.position = boardRect.TransformPoint(new Vector3(snapX, snapY, 0));

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            AttemptPlacement();
        }
    }

    public void Setup(Tile tile)
    {
        sourceTile = tile;
        abilityData = tile.ability;
        displayImage.sprite = tile.TileSprite;

        // Reset dimensions based on the sprite
        rect.sizeDelta = new Vector2(tile.TileSprite.rect.width, tile.TileSprite.rect.height);
        rect.localScale = Vector3.one;
    }

    // Keep OnDrag so the UI system knows you are interacting with it
    public void OnDrag(PointerEventData eventData) 
    {
        // We handle movement in Update, but this satisfies the Interface
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Handled by the Mouse Release check in Update
    }

    private void AttemptPlacement()
    {
        List<SkillBoardTileUI> overlappedUI = GetOverlappedTiles();

        if (overlappedUI.Count > 0 && IsValidMove(overlappedUI))
        {
            isSlotted = true; 

            // 1. SORT to find the Top-Left tile (The Origin)
            // We sort by Y (ascending) then X (ascending)
            overlappedUI.Sort((a, b) => {
                int res = a.boardPosition.y.CompareTo(b.boardPosition.y);
                if (res == 0) res = a.boardPosition.x.CompareTo(b.boardPosition.x);
                return res;
            });

            // 2. SNAP VISUALLY to the Origin tile
            transform.position = overlappedUI[0].transform.position;
            transform.SetAsLastSibling();

            // 3. DATA SAVE: Only the first tile gets the AbilityData
            Vector2Int originPos = overlappedUI[0].boardPosition;

            for (int i = 0; i < overlappedUI.Count; i++)
            {
                Vector2Int currentPos = overlappedUI[i].boardPosition;
                // Pass the sourceTile instead of just abilityData
                BoardInteractionManager.instance.activeBoard.SetTileAbility(
                    currentPos, 
                    (i == 0 ? sourceTile : null), 
                    originPos
                );
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private List<SkillBoardTileUI> GetOverlappedTiles()
    {
        List<SkillBoardTileUI> found = new List<SkillBoardTileUI>();

        int columns = Mathf.RoundToInt(rect.sizeDelta.x / 50f);
        int rows = Mathf.RoundToInt(rect.sizeDelta.y / 50f);

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                // 1. Calculate the center of each 50x50 grid cell relative to the TOP-LEFT pivot.
                // If x=0, segmentCenterX is 25. If x=1, it's 75.
                float segmentCenterX = (x * 50f) + 25f;
                
                // If y=0, segmentCenterY is -25. If y=1, it's -75.
                float segmentCenterY = -(y * 50f) - 25f;

                // 2. Transform this local "sticker point" into a World Screen Point
                Vector3 worldPoint = transform.TransformPoint(new Vector3(segmentCenterX, segmentCenterY, 0));

                // --- DEBUG VISUALIZER ---
                // This will show you exactly where the code is "feeling" for tiles.
                // Look for the red dots in your SCENE view while dragging.
                Debug.DrawRay(worldPoint, Vector3.forward * 100, Color.red, 0.1f);

                PointerEventData pointerData = new PointerEventData(EventSystem.current);
                pointerData.position = worldPoint;

                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);

                foreach (var hit in results)
                {
                    if (hit.gameObject == gameObject) continue;

                    SkillBoardTileUI tile = hit.gameObject.GetComponentInParent<SkillBoardTileUI>();
                    if (tile != null)
                    {
                        if (!found.Contains(tile)) found.Add(tile);
                        break; 
                    }
                }
            }
        }
        return found;
    }

    private bool IsOverlapping(RectTransform a, RectTransform b)
    {
        Vector3[] cornersA = new Vector3[4];
        Vector3[] cornersB = new Vector3[4];
        a.GetWorldCorners(cornersA);
        b.GetWorldCorners(cornersB);

        // Create the Rects
        Rect rectA = new Rect(cornersA[0].x, cornersA[0].y, cornersA[2].x - cornersA[0].x, cornersA[2].y - cornersA[0].y);
        Rect rectB = new Rect(cornersB[0].x, cornersB[0].y, cornersB[2].x - cornersB[0].x, cornersB[2].y - cornersB[0].y);

        // --- THE FIX: SHRINK THE SENSOR ---
        // Shrink the sprite's detection box by 5 pixels on each side 
        // so it doesn't "bleed" into neighboring tiles.
        float margin = 5f;
        Rect shrunkenA = new Rect(
            rectA.x + margin, 
            rectA.y + margin, 
            rectA.width - (margin * 2), 
            rectA.height - (margin * 2)
        );

        return shrunkenA.Overlaps(rectB);
    }

    private bool IsValidMove(List<SkillBoardTileUI> uiList)
    {
        // A 100x50 sprite MUST find exactly 2 tiles. 
        int expected = Mathf.RoundToInt(rect.sizeDelta.x / 50f) * Mathf.RoundToInt(rect.sizeDelta.y / 50f);
        
        if (uiList.Count != expected) 
        {
            Debug.Log($"Invalid: Found {uiList.Count} tiles, expected {expected}");
            return false;
        }

        foreach (var ui in uiList)
        {
            var data = BoardInteractionManager.instance.activeBoard.GetTileAt(ui.boardPosition);
            if (data == null || !data.isUnlocked || data.placedTile != null || !data.IsEmpty) 
            {
                Debug.Log($"Tile at {ui.boardPosition} is invalid (Locked/Occupied)");
                return false;
            }
        }
        return true;
    }
}