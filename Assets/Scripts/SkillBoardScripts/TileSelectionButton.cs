using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class TileSelectionButton : MonoBehaviour
{
    public TextMeshProUGUI TileName;
    public Image TileImage;
    public float ImageScale;

    public void Initialize(ItemStack item)
    {
        Tile tile = (Tile)item.item;
        TileName.text = tile.ItemName;
        TileImage.sprite = tile.ability.SkillTile;
        TileImage.SetNativeSize();
        TileImage.rectTransform.localScale = new Vector3(ImageScale, ImageScale, 1);
    }
}
