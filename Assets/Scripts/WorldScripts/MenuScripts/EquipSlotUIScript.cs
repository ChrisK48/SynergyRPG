using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipSlotUIScript : MonoBehaviour
{
    public TextMeshProUGUI EquipNameText;
    public Button btn;
    [HideInInspector] public Action onClicked;
    void Awake()
    {
        btn.onClick.AddListener(() => onClicked?.Invoke());
    }
    public void UpdateEquipSlot(Equippable newEquip)
    {
        EquipNameText.text = newEquip != null ? newEquip.ItemName : "Empty";
    }
}
