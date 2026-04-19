using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryMenuScript : MonoBehaviour
{
    public Button AllSortButton;
    public Button EquipSortButton;
    public Button GemSortButton;
    public Button ConsumableSortButton;
    public Button KeyItemSortButton;
    public GameObject ItemCard;
    public GridLayoutGroup InventoryLayout;
    public Button BackButton;

    void Start()
    {
        SetupSortButtons();
        CreateItemButtons(ItemType.All);
    }

    private void SetupSortButtons()
    {
        SetupButton(AllSortButton, ItemType.All);
        SetupButton(EquipSortButton, ItemType.Equippable);
        SetupButton(GemSortButton, ItemType.Gem);
        SetupButton(ConsumableSortButton, ItemType.Consumable);
        BackButton.onClick.AddListener(() => MenuManager.instance.InventoryContainer.SetActive(false));
    }

    private void SetupButton(Button sortButton, ItemType itemType)
    {
        sortButton.onClick.RemoveAllListeners();
        sortButton.onClick.AddListener(() => CreateItemButtons(itemType));
    }

    private void CreateItemButtons(ItemType itemType)
    {
        foreach (Transform child in InventoryLayout.transform) Destroy(child.gameObject);
        foreach (ItemStack itemStack in PartyManager.instance.inventory)
        {
            if (itemType == ItemType.All || itemStack.item.itemType == itemType)
            {
                GameObject card = Instantiate(ItemCard, InventoryLayout.transform);
                card.GetComponentInChildren<TextMeshProUGUI>().text = itemStack.item.ItemName + " x" + itemStack.count;
            }
        }
    }
}
