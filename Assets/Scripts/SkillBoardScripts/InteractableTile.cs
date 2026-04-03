using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class InteractableTile : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public TileItem sourceTile;
    public TileType tileData;
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
        // If it's already on the board, don't follow the mouse
        if (isSlotted) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        RectTransform boardRect = MenuManager.instance.SkillBoardContainer.GetComponentInChildren<GridLayoutGroup>().GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(boardRect, mousePos, null, out Vector2 localPoint);

        float snapX = Mathf.Floor(localPoint.x / 50f) * 50f;
        float snapY = Mathf.Ceil(localPoint.y / 50f) * 50f;

        transform.position = boardRect.TransformPoint(new Vector3(snapX, snapY, 0));

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            AttemptPlacement();
        }
    }

    public void Setup(TileItem tile)
    {
        sourceTile = tile;
        tileData = tile.tileType;
        displayImage.sprite = tile.TileSprite;
        displayImage.color = tile.tileColor;
        rect.sizeDelta = new Vector2(tile.TileSprite.rect.width, tile.TileSprite.rect.height);
        rect.localScale = Vector3.one;
    }

    // Required for OnBeginDrag to work on existing UI objects
    public void OnPointerDown(PointerEventData eventData) { }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Right Click to instantly delete
        if (eventData.button == PointerEventData.InputButton.Right && isSlotted)
        {
            SkillBoardTileUI closest = GetClosestTileUI();
            if (closest != null)
                BoardInteractionManager.instance.activeBoard.RemoveTileAt(closest.boardPosition);
            
            Destroy(gameObject);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isSlotted)
        {
            SkillBoardTileUI closest = GetClosestTileUI();
            if (closest != null)
            {
                // Remove from data so the slots are free again
                BoardInteractionManager.instance.activeBoard.RemoveTileAt(closest.boardPosition);
            }

            // Pick it back up
            isSlotted = false;
            transform.SetParent(BoardInteractionManager.instance.dragLayer);
            displayImage.raycastTarget = false; // Prevent it from blocking its own raycasts while dragging
        }
    }

    public void OnDrag(PointerEventData eventData) { /* Handled in Update */ }

    public void OnEndDrag(PointerEventData eventData)
    {
        displayImage.raycastTarget = true;
        AttemptPlacement();
    }

    private void AttemptPlacement()
    {
        List<SkillBoardTileUI> overlappedUI = GetOverlappedTiles();

        if (overlappedUI.Count > 0 && IsValidMove(overlappedUI))
        {
            isSlotted = true;
            overlappedUI.Sort((a, b) => {
                int res = a.boardPosition.y.CompareTo(b.boardPosition.y);
                if (res == 0) res = a.boardPosition.x.CompareTo(b.boardPosition.x);
                return res;
            });

            transform.SetParent(BoardInteractionManager.instance.placedLayer);
            transform.position = overlappedUI[0].transform.position;
            transform.SetAsLastSibling();

            Vector2Int originPos = overlappedUI[0].boardPosition;

            for (int i = 0; i < overlappedUI.Count; i++)
            {
                BoardInteractionManager.instance.activeBoard.SetTileAbility(
                    overlappedUI[i].boardPosition, 
                    (i == 0 ? sourceTile : null), 
                    originPos
                );
            }
            PartyManager.instance.SetTileEquipStatus(sourceTile, true);
        }
        else
        {
            // If we were dragging an existing tile and failed placement, it gets destroyed
            // (Or you could return it to the side list here)
            Destroy(gameObject);
        }
    }

    private SkillBoardTileUI GetClosestTileUI()
    {
        SkillBoardTileUI closest = null;
        float closestDist = float.MaxValue;
        foreach (var ui in BoardInteractionManager.instance.allTileUIs)
        {
            float dist = Vector2.Distance(transform.position, ui.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = ui;
            }
        }
        return closest;
    }

    public List<SkillBoardTileUI> GetOverlappedTiles()
    {
        List<SkillBoardTileUI> foundSlots = new List<SkillBoardTileUI>();

        // CHANGE: Raycast from the mouse position, NOT the tile's center
        // This ensures the pivotCoords match the slot your top-left corner is touching
        Vector2 mousePos = Mouse.current.position.ReadValue();
        SkillBoardTileUI pivotSlot = GetSlotAtPosition(mousePos);
        
        if (pivotSlot == null) return foundSlots;

        Vector2Int pivotCoords = pivotSlot.boardPosition;

        foreach (var offset in sourceTile.layoutOffsets)
        {
            Vector2Int targetCoords = pivotCoords + new Vector2Int(offset.x, offset.y);
            SkillBoardTileUI slot = BoardInteractionManager.instance.GetSlotAt(targetCoords);
            
            if (slot != null) foundSlots.Add(slot);
        }

        return foundSlots;
    }

    private bool IsValidMove(List<SkillBoardTileUI> uiList)
    {
        // Use the actual count of defined blocks, not the calculated area
        if (uiList.Count != sourceTile.layoutOffsets.Count) return false;

        foreach (var ui in uiList)
        {
            var data = BoardInteractionManager.instance.activeBoard.GetTileAt(ui.boardPosition);
            if (data == null || !data.isUnlocked || !data.IsEmpty) return false;
        }
        return true;
    }

    private SkillBoardTileUI GetSlotAtPosition(Vector3 worldPos)
    {
        // Create a pointer event for the UI Raycaster
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = worldPos;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            SkillBoardTileUI slot = result.gameObject.GetComponent<SkillBoardTileUI>();
            if (slot != null) return slot;
        }

        return null;
    }
}