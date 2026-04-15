using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.UI;
using System.Collections;
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
        WeaponContainer.GetComponentInChildren<EquipSlotUIScript>().Setup(currentChar.weaponSlot.currentItem, currentChar.weaponSlot);
        ArmorContainer.GetComponentInChildren<EquipSlotUIScript>().Setup(currentChar.armorSlot.currentItem, currentChar.armorSlot);
        AccessoryContainer.GetComponentInChildren<EquipSlotUIScript>().Setup(currentChar.accessorySlot.currentItem, currentChar.accessorySlot);
        WeaponContainer.GetComponentInChildren<EquipSlotUIScript>().openEquipSelection = OpenEquipSelection;
        ArmorContainer.GetComponentInChildren<EquipSlotUIScript>().openEquipSelection = OpenEquipSelection;
        AccessoryContainer.GetComponentInChildren<EquipSlotUIScript>().openEquipSelection = OpenEquipSelection;
        WeaponContainer.GetComponentInChildren<EquipSlotUIScript>().openGemSelection = (slotIndex) => OpenGemSelection(currentChar.weaponSlot, slotIndex);
        ArmorContainer.GetComponentInChildren<EquipSlotUIScript>().openGemSelection = (slotIndex) => OpenGemSelection(currentChar.armorSlot, slotIndex);
        AccessoryContainer.GetComponentInChildren<EquipSlotUIScript>().openGemSelection = (slotIndex) => OpenGemSelection(currentChar.accessorySlot, slotIndex);
        WeaponContainer.GetComponentInChildren<EquipSlotUIScript>().Refresh = () => { currentChar.RefreshAllStats(); currentChar.RefreshAbilities(); };
        ArmorContainer.GetComponentInChildren<EquipSlotUIScript>().Refresh = () => { currentChar.RefreshAllStats(); currentChar.RefreshAbilities(); };
        AccessoryContainer.GetComponentInChildren<EquipSlotUIScript>().Refresh = () => { currentChar.RefreshAllStats(); currentChar.RefreshAbilities(); };
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
            if (itemStack.item is Equippable equippable)
            {
                var entry = PartyManager.instance.equipList.Find(e => e.Item2 == equippable);
                if (entry != null)
                {
                    if (entry.Item1 == currentChar)
                    {
                        btn.GetComponentInChildren<TextMeshProUGUI>().text += " (Currently Equipped)";
                    }
                    else
                    {
                        btn.GetComponentInChildren<TextMeshProUGUI>().text += " (Equipped by " + entry.Item1.CharName + ")";
                        btn.onClick.AddListener(() => {
                            EquipSlot equippedSlot = entry.Item2.equipSlot;
                            entry.Item1.GetEquipSlot(equippedSlot).Unequip(entry.Item1, entry.Item2);
                            entry.Item1.GetEquipSlot(equippedSlot).equippedGems.Clear();
                            entry.Item1.RefreshAllStats();
                            entry.Item1.RefreshAbilities();
                            EquipItem(equippable);
                        });
                    }
                }
                else
                {
                    btn.onClick.AddListener(() => EquipItem(equippable));
                }
            }
        }
    }

    private void EquipItem(Equippable equipItem)
    {
        switch (equipItem.equipSlot)
        {
            case EquipSlot.Weapon:
                currentChar.weaponSlot.Equip(currentChar,equipItem);
                break;
            case EquipSlot.Armor:
                currentChar.armorSlot.Equip(currentChar, equipItem);
                break;
            case EquipSlot.Accessory:
                currentChar.accessorySlot.Equip(currentChar, equipItem);
                break;
        }
        SetupEquipSlots();
        EquipSelectionContainer.SetActive(false);
    }

    private void EquipGem(Gem gem, EquipmentSlot equipmentSlot, int gemSlotIndex)
    {
        equipmentSlot.EquipGem(gem, gemSlotIndex, currentChar);
        SetupEquipSlots();
        EquipSelectionContainer.SetActive(false);
    }

    public void OpenGemSelection(EquipmentSlot equipmentSlot, int gemSlotIndex)
    {
        EquipSelectionContainer.SetActive(true);
        foreach (Transform child in EquipSelectionContainer.transform) Destroy(child.gameObject);

        foreach (ItemStack itemStack in PartyManager.instance.inventory.Where(stack => stack.item is Gem))
        {
            Button btn = Instantiate(EquipButton, EquipSelectionContainer.transform);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = itemStack.item.ItemName + " x" + itemStack.count;
            if (itemStack.item is Gem gem)
            {
                btn.onClick.AddListener(() => 
                { 
                    EquipGem(gem, equipmentSlot, gemSlotIndex); 
                    currentChar.RefreshAllStats();
                    currentChar.RefreshAbilities();
                });
            }
        }
    }
}
