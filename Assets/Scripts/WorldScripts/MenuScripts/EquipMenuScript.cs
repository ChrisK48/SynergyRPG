using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.UI;
public class EquipMenuScript : MonoBehaviour
{
    public GameObject WeaponContainer;
    public GameObject ArmorContainer;
    public GameObject AccessoryContainer;
    public GameObject PartyMemberVisualContainer;
    public GameObject MemberContainer;
    public GameObject EquipSelectionContainer;
    public Button EquipButton;
    private PlayerCharData[] unlockedPartyMembers;
    private PlayerCharData currentChar;
    private int currentCharIndex = 0;

    void Awake()
    {
        unlockedPartyMembers = Resources.LoadAll<PlayerCharData>("Data/PartyStats");
        currentChar = unlockedPartyMembers[currentCharIndex];
        Debug.Log($"Current char in equip menu: {currentChar.CharName}");
    }

    void Start()
    {
        SwapInPartyMember();
        SetupEquipSlots();
    }

    void Update()
    {
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            if (currentCharIndex == unlockedPartyMembers.Length - 1)
            {
                currentCharIndex = 0;
            }
            else
            {
                currentCharIndex++;
            }
            currentChar = unlockedPartyMembers[currentCharIndex];
            SwapInPartyMember();
            SetupEquipSlots();
        }
        else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            if (currentCharIndex == 0)
            {
                currentCharIndex = unlockedPartyMembers.Length - 1;
            }
            else
            {
                currentCharIndex--;
            }
            currentChar = unlockedPartyMembers[currentCharIndex];
            SwapInPartyMember();
            SetupEquipSlots();
        }
    }

    private void SetupEquipSlots()
    {
        WeaponContainer.GetComponentInChildren<EquipSlotUIScript>().UpdateEquipSlot(currentChar.weapon);
        ArmorContainer.GetComponentInChildren<EquipSlotUIScript>().UpdateEquipSlot(currentChar.armor);
        AccessoryContainer.GetComponentInChildren<EquipSlotUIScript>().UpdateEquipSlot(currentChar.accessory);
        WeaponContainer.GetComponentInChildren<EquipSlotUIScript>().onClicked = () => OpenEquipSelection();
        ArmorContainer.GetComponentInChildren<EquipSlotUIScript>().onClicked = () => OpenEquipSelection();
        AccessoryContainer.GetComponentInChildren<EquipSlotUIScript>().onClicked = () => OpenEquipSelection();
    }

    private void SwapInPartyMember()
    {
        if (PartyMemberVisualContainer.transform.childCount > 0)
        {
            foreach (Transform child in PartyMemberVisualContainer.transform)
            {
                Destroy(child.gameObject);
            }
        }

        GameObject clone = Instantiate(MemberContainer, PartyMemberVisualContainer.transform);
        clone.GetComponent<PartyMemberUI>().Initialize(currentChar);
    }

    private void OpenEquipSelection()
    {
        EquipSelectionContainer.SetActive(true);
        foreach (Transform child in EquipSelectionContainer.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (ItemStack itemStack in PartyManager.instance.inventory.Where(stack => stack.item is Equippable))
        {
            Button btn = Instantiate(EquipButton, EquipSelectionContainer.transform);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = itemStack.item.ItemName + " x" + itemStack.count;
            btn.onClick.AddListener(() => EquipItem(itemStack));
        }
    }

    private void EquipItem(ItemStack itemStack)
    {
        Equippable equipItem = itemStack.item as Equippable;
        switch (equipItem.equipSlot)
        {
            case EquipSlot.Weapon:
                currentChar.weapon = equipItem;
                break;
            case EquipSlot.Armor:
                currentChar.armor = equipItem;
                break;
            case EquipSlot.Accessory:
                currentChar.accessory = equipItem;
                break;
        }
        SetupEquipSlots();
        EquipSelectionContainer.SetActive(false);
    }
}
