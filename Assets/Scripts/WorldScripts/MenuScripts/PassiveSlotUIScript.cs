using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class PassiveSlotUIScript : MonoBehaviour, IPointerClickHandler
{
    public Image SlotImage;
    public Button SlotButton;
    public int slotIndex;
    [HideInInspector] public Action<PassiveSlotUIScript> OpenPassiveSelection;
    private CharPassive passive;
    private PlayerCharData currentChar;

    void Awake()
    {
        SlotButton.onClick.AddListener(() => OpenPassiveSelection?.Invoke(this));
    }
    public void Setup(PlayerCharData currentChar, Action<PassiveSlotUIScript> openPassiveSelection)
    {
        OpenPassiveSelection = openPassiveSelection;
        this.currentChar = currentChar;
        passive = currentChar.passiveSlots[slotIndex];
        if (passive != null) SlotImage.sprite = passive.passiveSprite;
    }

    public void EquipPassive(CharPassive newPassive)
    {
        passive = newPassive;
        currentChar.passiveSlots[slotIndex] = newPassive;
        if (passive != null) SlotImage.sprite = passive.passiveSprite;
    }

    public void UnequipPassive()
    {
        passive = null;
        currentChar.passiveSlots[slotIndex] = null;
        SlotImage.sprite = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            UnequipPassive();
        }
    }
}
