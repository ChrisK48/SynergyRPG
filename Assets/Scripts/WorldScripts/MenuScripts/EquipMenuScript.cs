using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
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
        WeaponContainer.GetComponentInChildren<EquipSlotUIScript>().openGemSelection = (gemSlotUI) => OpenGemSelection(currentChar.weaponSlot, gemSlotUI);
        ArmorContainer.GetComponentInChildren<EquipSlotUIScript>().openGemSelection = (gemSlotUI) => OpenGemSelection(currentChar.armorSlot, gemSlotUI);
        AccessoryContainer.GetComponentInChildren<EquipSlotUIScript>().openGemSelection = (gemSlotUI) => OpenGemSelection(currentChar.accessorySlot, gemSlotUI);
        WeaponContainer.GetComponentInChildren<EquipSlotUIScript>().Refresh = () => { currentChar.RefreshAllStats(); currentChar.RefreshAbilities(); };
        ArmorContainer.GetComponentInChildren<EquipSlotUIScript>().Refresh = () => { currentChar.RefreshAllStats(); currentChar.RefreshAbilities(); };
        AccessoryContainer.GetComponentInChildren<EquipSlotUIScript>().Refresh = () => { currentChar.RefreshAllStats(); currentChar.RefreshAbilities(); };
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

    foreach (ItemStack itemStack in PartyManager.instance.inventory.Where(stack => stack.item is Equippable))
    {
        Button btn = Instantiate(EquipButton, EquipSelectionContainer.transform);
        btn.GetComponentInChildren<TextMeshProUGUI>().text = itemStack.item.ItemName + " x" + itemStack.count;

        if (itemStack.item is Equippable equippable)
        {
            // 1. Calculate the truth: Who is currently wearing this specific equippable asset?
            var equippedEntries = PartyManager.instance.unlockedPartyMembers
                .SelectMany(charData => new[] { 
                    new { charData, slot = charData.weaponSlot }, 
                    new { charData, slot = charData.armorSlot }, 
                    new { charData, slot = charData.accessorySlot } 
                })
                .Where(x => x.slot.currentItem == equippable)
                .ToList();

            int equippedCount = equippedEntries.Count;
            Debug.Log($"Equipped count for {equippable.ItemName}: {equippedCount}");
            bool isEquippedByCurrent = equippedEntries.Any(e => e.charData == currentChar);

            if (isEquippedByCurrent)
            {
                btn.GetComponentInChildren<TextMeshProUGUI>().text += " (Currently Equipped)";
            }
            else if (equippedCount >= itemStack.count && equippedCount > 0)
            {
                // 2. All copies are in use. Show who has them.
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
                            // Strip item and gems from target
                            entry.slot.Unequip(entry.charData, equippable);
                            entry.slot.equippedGems.Clear();
                            
                            entry.charData.RefreshAllStats();
                            entry.charData.RefreshAbilities();

                            SwapEquipContainer.SetActive(false);
                            EquipItem(equippable);
                        });
                    }
                });
            }
            else
            {
                // 3. Available in inventory
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
                currentChar.weaponSlot.Unequip(currentChar, currentChar.weaponSlot.currentItem);
                break;
            case EquipSlot.Armor:
                currentChar.armorSlot.Unequip(currentChar, currentChar.armorSlot.currentItem);
                break;
            case EquipSlot.Accessory:
                currentChar.accessorySlot.Unequip(currentChar, currentChar.accessorySlot.currentItem);
                break;
        }

        currentChar.GetEquipSlot(slot).equippedGems.Clear();
        currentChar.RefreshAllStats();
        currentChar.RefreshAbilities();
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

        foreach (ItemStack itemStack in PartyManager.instance.inventory.Where(stack => stack.item is Gem && ((Gem)stack.item).GemType == gemSlotUI.slotType))
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
                    // Optional: You could still allow re-equipping to a different slot if you want
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
                            
                            SwapEquipContainer.SetActive(false);
                            EquipGem(gem, equipmentSlot, gemSlotUI.slotIndex);
                            });
                        }
                    });
                } else if (equippedCount > 0)
                {
                    // 2. If some but not all owned gems are in use, indicate that and allow equipping
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
                    });
                }
            }
        }
    }
}
