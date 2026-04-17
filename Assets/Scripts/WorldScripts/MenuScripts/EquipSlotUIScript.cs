using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipSlotUIScript : MonoBehaviour, IPointerClickHandler
{
    public TextMeshProUGUI EquipNameText;
    public GameObject GemSlotContainer;
    public GameObject GemSlot;
    public Button SwapItemButton;
    private Equippable currentEquip;
    private EquipmentSlot equipSlot;
    [HideInInspector] public Action openEquipSelection;
    [HideInInspector] public Action<GemSlotUI> openGemSelection;
    [HideInInspector] public Action<EquipSlot> UnequipItem;
    [HideInInspector] public Action Refresh;

    void Awake()
    {
        SwapItemButton.onClick.AddListener(() => openEquipSelection?.Invoke());
    }

    public void Setup(Equippable newEquip, EquipmentSlot equipSlot)
    {
        this.equipSlot = equipSlot;
        UpdateEquipSlot(newEquip);
    }

    public void UpdateEquipSlot(Equippable newEquip)
    {
        currentEquip = newEquip;
        EquipNameText.text = newEquip != null ? newEquip.ItemName : "Empty";
        PopulateGemSlots();
    }

    private void PopulateGemSlots()
    {
        foreach (Transform child in GemSlotContainer.transform)
        {
            Destroy(child.gameObject);
        }

        if (currentEquip == null) return;
        for (int i = 0; i < currentEquip.gemSlots.Count; i++)
        {
            int slotIndex = i;
            GameObject newGemSlot = Instantiate(GemSlot, GemSlotContainer.transform);
            
            SetSlotColor(newGemSlot, slotIndex);

            GemSlotUI gemSlotUI = newGemSlot.GetComponent<GemSlotUI>();
            gemSlotUI.Setup(equipSlot.equippedGems[i]);
            gemSlotUI.slotType = currentEquip.gemSlots[i].slotType;
            gemSlotUI.slotIndex = slotIndex;
            newGemSlot.GetComponentInChildren<Button>().onClick.AddListener(() => openGemSelection?.Invoke(gemSlotUI));
            gemSlotUI.onRightClick = (index) => RemoveGemFromSlot(index);
        }
    }

    private void RemoveGemFromSlot(int slotIndex)
    {
        if (equipSlot != null && equipSlot.equippedGems[slotIndex] != null)
        {
            Debug.Log($"Removing gem from slot {slotIndex + 1} of {currentEquip.ItemName}");            
            equipSlot.equippedGems[slotIndex] = null;

            PopulateGemSlots();
            Refresh?.Invoke();
        }
    }

    private void SetSlotColor(GameObject slot, int slotIndex)
    {
        Image slotImage = slot.GetComponent<Image>();
        if (slotImage != null)
        {
            switch (currentEquip.gemSlots[slotIndex].slotType)
            {
                case GemType.Mira:
                    slotImage.color = Color.green;
                    break;
                case GemType.Rain:
                    slotImage.color = Color.lightBlue;
                    break;
                case GemType.Gladus:
                    slotImage.color = Color.blue;
                    break;
                case GemType.Eldric:
                    slotImage.color = Color.purple;
                    break;
                case GemType.GeneralAbility:
                    slotImage.color = Color.yellow;
                    break;
                default:
                    slotImage.color = Color.white;
                    break;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            UnequipItem?.Invoke(equipSlot.slotType);
        }
    }
}
