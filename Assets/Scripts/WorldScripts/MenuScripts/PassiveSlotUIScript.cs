using UnityEngine.UI;
using UnityEngine;
using System;

public class PassiveSlotUIScript : MonoBehaviour
{
    public Image SlotImage;
    public Button SlotButton;
    public int slotIndex;
    [HideInInspector] public Action OpenPassiveSelection;
    private CharPassive passive;
    private PlayerCharData currentChar;

    void Awake()
    {
        SlotButton.onClick.AddListener(() => OpenPassiveSelection?.Invoke());
    }
    public void Setup(PlayerCharData currentChar, Action openPassiveSelection)
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
}
