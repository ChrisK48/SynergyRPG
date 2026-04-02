using UnityEngine;

[CreateAssetMenu(fileName = "NewTile", menuName = "Item/Tile")]
public class Tile : Item
{
    public Ability ability;
    public Sprite TileSprite;
    public bool isExclusive;
}
