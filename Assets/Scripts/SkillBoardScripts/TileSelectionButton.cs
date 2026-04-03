using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TileSelectionButton : MonoBehaviour
{
    public TextMeshProUGUI TileName;
    public Image TileImage;
    private TileItem myTile; // Reference to the Tile ScriptableObject

    public void Initialize(InventoryEntry stack)
    {
        myTile = (TileItem)stack.item; 
        
        TileName.text = myTile.ItemName;
        TileImage.sprite = myTile.TileSprite;
        TileImage.color = myTile.tileColor;
        TileImage.SetNativeSize();
    }

    public void OnClick()
    {
        BoardInteractionManager.instance.SpawnSticker(myTile);
    }
}