using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.UI;
using Unity.VisualScripting;

public class EquipMenuScript : MonoBehaviour
{
    public GameObject WeaponContainer;
    public GameObject ArmorContainer;
    public GameObject AccessoryContainer;
    public GameObject PartyMemberVisualContainer;
    public GameObject MemberContainer;
    public GameObject EquipSelectionContainer;
    public GameObject SwapEquipContainer;
    public Button EquipButton;
    public Button BackButton;
    private PlayerCharData currentChar;
    private int currentCharIndex = 0;

    void Awake()
    {
        currentChar = PartyManager.instance.unlockedPartyMembers[currentCharIndex];
        Debug.Log($"Current char in equip menu: {currentChar.CharName}");
    }

    void Start()
    {
        SwapInPartyMember();
        SetupEquipSlots();
        BackButton.onClick.AddListener(() => MenuManager.instance.EquipmentContainer.SetActive(false));
    }

    void Update()
    {
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            if (currentCharIndex == PartyManager.instance.unlockedPartyMembers.Count - 1)
            {
                currentCharIndex = 0;
            }
            else
            {
                currentCharIndex++;
            }
            currentChar = PartyManager.instance.unlockedPartyMembers[currentCharIndex];
            SwapInPartyMember();
            SetupEquipSlots();
            EquipSelectionContainer.SetActive(false);
        }
        else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            if (currentCharIndex == 0)
            {
                currentCharIndex = PartyManager.instance.unlockedPartyMembers.Count - 1;
            }
            else
            {
                currentCharIndex--;
            }
            currentChar = PartyManager.instance.unlockedPartyMembers[currentCharIndex];
            SwapInPartyMember();
            SetupEquipSlots();
            EquipSelectionContainer.SetActive(false);
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
        WeaponContainer.GetComponentInChildren<EquipSlotUIScript>().openGemSelection = (gemSlotUI) => OpenGemSelection(currentChar.weaponSlot, gemSlotUI);
        ArmorContainer.GetComponentInChildren<EquipSlotUIScript>().openGemSelection = (gemSlotUI) => OpenGemSelection(currentChar.armorSlot, gemSlotUI);
        AccessoryContainer.GetComponentInChildren<EquipSlotUIScript>().openGemSelection = (gemSlotUI) => OpenGemSelection(currentChar.accessorySlot, gemSlotUI);
        WeaponContainer.GetComponentInChildren<EquipSlotUIScript>().UnequipItem = (slot) => UnequipItem(slot);
        ArmorContainer.GetComponentInChildren<EquipSlotUIScript>().UnequipItem = (slot) => UnequipItem(slot);
        AccessoryContainer.GetComponentInChildren<EquipSlotUIScript>().UnequipItem = (slot) => UnequipItem(slot);
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
        foreach (Transform child in EquipSelectionContainer.transform) Destroy(child.gameObject);

        foreach (ItemStack itemStack in PartyManager.instance.inventory.Where(stack => stack.item is Equippable equip && IsMatching(stack)))
        {
            Button btn = Instantiate(EquipButton, EquipSelectionContainer.transform);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = itemStack.item.ItemName + " x" + itemStack.count;

            if (itemStack.item is Equippable equippable)
            {
                var equippedEntries = PartyManager.instance.unlockedPartyMembers
                    .SelectMany(charData => new[] { 
                        new { charData, slot = charData.weaponSlot }, 
                        new { charData, slot = charData.armorSlot }, 
                        new { charData, slot = charData.accessorySlot } 
                    })
                    .Where(x => x.slot.currentItem == equippable)
                    .ToList();

                int equippedCount = equippedEntries.Count;
                bool isEquippedByCurrent = equippedEntries.Any(e => e.charData == currentChar);

                if (isEquippedByCurrent)
                {
                    btn.GetComponentInChildren<TextMeshProUGUI>().text += " (Currently Equipped)";
                }
                else if (equippedCount >= itemStack.count && equippedCount > 0)
                {
                    string names = string.Join(", ", equippedEntries.Select(e => e.charData.CharName));
                    btn.GetComponentInChildren<TextMeshProUGUI>().text += $" (Equipped by {names})";

                    btn.onClick.AddListener(() => {
                        SwapEquipContainer.SetActive(true);
                        foreach (Transform child in SwapEquipContainer.transform) Destroy(child.gameObject);

                        foreach (var entry in equippedEntries)
                        {
                            Button swapBtn = Instantiate(EquipButton, SwapEquipContainer.transform);
                            swapBtn.GetComponentInChildren<TextMeshProUGUI>().text = $"Swap with {entry.charData.CharName}";

                            swapBtn.onClick.AddListener(() => {
                                entry.slot.Unequip(entry.charData);
                                SwapEquipContainer.SetActive(false);
                                EquipItem(equippable);
                            });
                        }
                    });
                }
                else
                {
                    if (equippedCount > 0)
                    {
                        btn.GetComponentInChildren<TextMeshProUGUI>().text += $" ({equippedCount} of {itemStack.count} Equipped)";
                    }
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

    private void UnequipItem(EquipSlot slot)
    {
        switch (slot)
        {
            case EquipSlot.Weapon:
                currentChar.weaponSlot.Unequip(currentChar);
                break;
            case EquipSlot.Armor:
                currentChar.armorSlot.Unequip(currentChar);
                break;
            case EquipSlot.Accessory:
                currentChar.accessorySlot.Unequip(currentChar);
                break;
        }

        currentChar.GetEquipSlot(slot).equippedGems.Clear();
        currentChar.RefreshAllStats();
        currentChar.RefreshAbilities();
        PartyManager.instance.UpdateSynergies();
        SetupEquipSlots();
    }

    private void EquipGem(Gem gem, EquipmentSlot equipmentSlot, int gemSlotIndex)
    {
        equipmentSlot.EquipGem(gem, gemSlotIndex, currentChar);
        SetupEquipSlots();
        EquipSelectionContainer.SetActive(false);
    }

    public void OpenGemSelection(EquipmentSlot equipmentSlot, GemSlotUI gemSlotUI)
    {
        EquipSelectionContainer.SetActive(true);
        foreach (Transform child in EquipSelectionContainer.transform) Destroy(child.gameObject);

        foreach (ItemStack itemStack in PartyManager.instance.inventory.Where(stack => stack.item is Gem gem && gem.GemType == gemSlotUI.slotType && IsMatching(stack)))
        {
            Button btn = Instantiate(EquipButton, EquipSelectionContainer.transform);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = itemStack.item.ItemName + " x" + itemStack.count;

            if (itemStack.item is Gem gem)
            {
                var equippedEntries = PartyManager.instance.unlockedPartyMembers
                    .SelectMany(charData => new[] { 
                        new { charData, slot = charData.weaponSlot }, 
                        new { charData, slot = charData.armorSlot }, 
                        new { charData, slot = charData.accessorySlot } 
                    })
                    .SelectMany(x => x.slot.equippedGems.Select((gem, index) => new { x.charData, x.slot, gem, index }))
                    .Where(x => x.gem == gem)
                    .ToList();
                int equippedCount = equippedEntries.Count();

                bool isEquippedByCurrent = equippedEntries.Any(e => e.charData == currentChar);

                if (isEquippedByCurrent)
                {
                    btn.GetComponentInChildren<TextMeshProUGUI>().text += " (Currently Equipped)";
                }
                else if (equippedCount >= itemStack.count && equippedCount > 0)
                {
                    btn.GetComponentInChildren<TextMeshProUGUI>().text += " (Equipped by ";
                    foreach (var entry in equippedEntries) btn.GetComponentInChildren<TextMeshProUGUI>().text += entry.charData.CharName + ", ";
                    btn.GetComponentInChildren<TextMeshProUGUI>().text = btn.GetComponentInChildren<TextMeshProUGUI>().text.TrimEnd(',', ' ') + ")";

                    btn.onClick.AddListener(() => {
                        SwapEquipContainer.SetActive(true);
                        foreach (Transform child in SwapEquipContainer.transform) Destroy(child.gameObject);
                        foreach (var entry in equippedEntries)
                        {
                            Button swapBtn = Instantiate(EquipButton, SwapEquipContainer.transform);
                            swapBtn.GetComponentInChildren<TextMeshProUGUI>().text = $"Swap with {entry.charData.CharName}";

                            swapBtn.onClick.AddListener(() => {
                            entry.charData.GetEquipSlot(equipmentSlot.slotType).equippedGems[entry.index] = null;
                            
                            entry.charData.RefreshAllStats();
                            entry.charData.RefreshAbilities();
                            PartyManager.instance.UpdateSynergies();
                            
                            SwapEquipContainer.SetActive(false);
                            EquipGem(gem, equipmentSlot, gemSlotUI.slotIndex);
                            });
                        }
                    });
                } else if (equippedCount > 0)
                {
                    btn.GetComponentInChildren<TextMeshProUGUI>().text += $" ({equippedCount} of {itemStack.count} Equipped)";
                    btn.onClick.AddListener(() => EquipGem(gem, equipmentSlot, gemSlotUI.slotIndex));
                }
                else
                {
                    btn.onClick.AddListener(() => 
                    { 
                        btn.GetComponentInChildren<TextMeshProUGUI>().text += "x" + itemStack.count;
                        EquipGem(gem, equipmentSlot, gemSlotUI.slotIndex); 
                        currentChar.RefreshAllStats();
                        currentChar.RefreshAbilities();
                        PartyManager.instance.UpdateSynergies();
                    });
                }
            }
        }
    }

    public bool IsMatching(ItemStack itemStack)
    {
        PlayerID? exclusive = itemStack.item switch
        {
            Equippable equip => equip.charExclusive,
            Gem gem          => gem.charExclusive,
            _                => null
        };

        return exclusive.HasValue && (exclusive == PlayerID.Any || exclusive == currentChar.playerID);
    }
}
