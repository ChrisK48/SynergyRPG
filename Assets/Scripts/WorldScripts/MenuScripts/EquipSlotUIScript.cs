using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UI;

public class EquipSlotUIScript : MonoBehaviour
{
    public TextMeshProUGUI EquipNameText;
    public GameObject GemSlotContainer;
    public GameObject GemSlot;
    public Button SwapItemButton;
    private Equippable currentEquip;
    private EquipmentSlot equipSlot;
    [HideInInspector] public Action openEquipSelection;
    [HideInInspector] public Action<int> openGemSelection;
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
            GemSlotUI gemSlotUI = newGemSlot.GetComponent<GemSlotUI>();
            gemSlotUI.Setup(equipSlot.equippedGems[i]);
            gemSlotUI.slotIndex = slotIndex;
            newGemSlot.GetComponentInChildren<Button>().onClick.AddListener(() => openGemSelection?.Invoke(slotIndex));
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
}
