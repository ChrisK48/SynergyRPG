using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TileSelectionButton : MonoBehaviour
{
    public TextMeshProUGUI TileName;
    public Image TileImage;
    private Tile myTile; // Reference to the Tile ScriptableObject

    public void Initialize(ItemStack stack)
    {
        myTile = (Tile)stack.item; 
        
        TileName.text = myTile.ItemName;
        TileImage.sprite = myTile.TileSprite;
        TileImage.SetNativeSize();
    }

    public void OnClick()
    {
        BoardInteractionManager.instance.SpawnSticker(myTile);
    }
}