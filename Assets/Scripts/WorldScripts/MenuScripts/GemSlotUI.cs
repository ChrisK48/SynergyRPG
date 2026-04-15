using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class GemSlotUI : MonoBehaviour, IPointerClickHandler
{
    public Button GemButton;
    public Image GemImage;
    [HideInInspector] public int slotIndex;
    [HideInInspector] public Action<int> onRightClick;
    private Gem currentGem;

    public void Setup(Gem gem)
    {
        currentGem = gem;
        GemImage.sprite = gem != null ? gem.GemSprite : null;
        GemImage.color = gem != null ? gem.GemColor : Color.clear;
        GemImage.enabled = gem != null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            onRightClick?.Invoke(slotIndex);
        }
    }
}
