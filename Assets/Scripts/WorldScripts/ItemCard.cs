using TMPro;
using UnityEngine;

public class ItemCard : MonoBehaviour
{
    public TextMeshProUGUI ItemText;
    public void Initialize(InventoryEntry itemStack)
    {
        ItemText.text = itemStack.item.ItemName + " x" + itemStack.count;
    }
}
