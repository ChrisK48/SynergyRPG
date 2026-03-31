using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    [Header("Main Menu")]
    public GameObject MenuPanel;
    public Button MapButton;
    public Button BeastiaryButton;
    public Button InventoryButton;
    public Button EquipmentButton;
    public Button SkillButton;
    public Button ConfigButton;
    public GameObject PartyMembersContainer;
    public GameObject PartyMemberUIPrefab;

    [Header("Inventory Menu")]
    public GameObject InventoryContainer;
    public GameObject ItemCardPrefab;

    [Header("Equipment Menu")]
    public GameObject EquipmentContainer;
    public GameObject PlayerSelectContainer;
    public Button WeaponSelectButton;
    public Button HelmetSelectButton;
    public Button BodySelectButton;
    public Button Accessory1SelectButton;
    public Button Accessory2SelectButton;
    public Button PlayerNameButtonPrefab;
    public GameObject EquipmentSelectionPanel;
    public Button EquipmentButtonPrefab;
    private bool menuOpen = false;
    private bool inventoryOpen = false;
    private bool statsDisplayed = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {

    }

    void Start()
    {
        BuildActiveMemberCards();
        InventoryButton.onClick.AddListener(BuildInventoryMenu);
        EquipmentButton.onClick.AddListener(BuildEquipmentMenu);
    }

    public void ToggleMenu()
    {
        menuOpen = !menuOpen;
        MenuPanel.SetActive(menuOpen);
        if (inventoryOpen)
        {
            InventoryContainer.SetActive(false);
            inventoryOpen = false;
        }
        if (EquipmentContainer.activeSelf)
        {
            EquipmentContainer.SetActive(false);
        }
    }

    public bool GetIfMenuOpen() { return menuOpen; }

    private void BuildActiveMemberCards()
    {
        ClearCards();
        foreach (var member in PartyManager.instance.activePartyMembers)
        {
            GameObject card = Instantiate(PartyMemberUIPrefab, PartyMembersContainer.transform);
            card.GetComponent<PartyMemberUI>().Initialize(member);
        }
    }

    private void BuildInventoryMenu()
    {
        foreach (Transform child in InventoryContainer.transform)
        {
            Destroy(child.gameObject);
        }

        InventoryContainer.SetActive(true);
        inventoryOpen = true;
        foreach(ItemStack item in PartyManager.instance.inventory)
        {
            GameObject itemCard = Instantiate(ItemCardPrefab, InventoryContainer.transform);
            itemCard.GetComponent<ItemCard>().Initialize(item);
        }
    }

    private void BuildEquipmentMenu()
    {
        EquipmentContainer.SetActive(true);
        foreach(PlayerCharData member in Resources.LoadAll<PlayerCharData>("Data/PartyStats"))
        {
            Button btn = Instantiate(PlayerNameButtonPrefab, PlayerSelectContainer.transform);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = member.CharName;
            btn.GetComponent<Button>().onClick.AddListener(() => {
                PlayerCharData selectedMember = member;
                SetupEquipmentSelectionButtons(selectedMember);
            });
        }
    }

    private void SetupEquipmentSelectionButtons(PlayerCharData member)
    {
        WeaponSelectButton.GetComponentInChildren<TextMeshProUGUI>().text = member.weapon?.ItemName ?? "None";
        HelmetSelectButton.GetComponentInChildren<TextMeshProUGUI>().text = member.head?.ItemName ?? "None";
        BodySelectButton.GetComponentInChildren<TextMeshProUGUI>().text = member.body?.ItemName ?? "None";
        Accessory1SelectButton.GetComponentInChildren<TextMeshProUGUI>().text = member.accessory1?.ItemName ?? "None";
        Accessory2SelectButton.GetComponentInChildren<TextMeshProUGUI>().text = member.accessory2?.ItemName ?? "None";

        WeaponSelectButton.onClick.RemoveAllListeners();
        HelmetSelectButton.onClick.RemoveAllListeners();
        BodySelectButton.onClick.RemoveAllListeners();
        Accessory1SelectButton.onClick.RemoveAllListeners();
        Accessory2SelectButton.onClick.RemoveAllListeners();

        WeaponSelectButton.onClick.AddListener(() => { OpenEquipmentSelection(member, EquipSlot.Weapon); EquipmentSelectionPanel.transform.position = new Vector2(EquipmentSelectionPanel.transform.position.x, WeaponSelectButton.transform.position.y); });
        HelmetSelectButton.onClick.AddListener(() => { OpenEquipmentSelection(member, EquipSlot.Head); EquipmentSelectionPanel.transform.position = new Vector2(EquipmentSelectionPanel.transform.position.x, HelmetSelectButton.transform.position.y); });
        BodySelectButton.onClick.AddListener(() => { OpenEquipmentSelection(member, EquipSlot.Body); EquipmentSelectionPanel.transform.position = new Vector2(EquipmentSelectionPanel.transform.position.x, BodySelectButton.transform.position.y); });
        Accessory1SelectButton.onClick.AddListener(() => { OpenEquipmentSelection(member, EquipSlot.Accessory); EquipmentSelectionPanel.transform.position = new Vector2(EquipmentSelectionPanel.transform.position.x, Accessory1SelectButton.transform.position.y); });
        Accessory2SelectButton.onClick.AddListener(() => { OpenEquipmentSelection(member, EquipSlot.Accessory); EquipmentSelectionPanel.transform.position = new Vector2(EquipmentSelectionPanel.transform.position.x, Accessory2SelectButton.transform.position.y); });
    }

    private void OpenEquipmentSelection(PlayerCharData member,  EquipSlot slot)
    {
        EquipmentSelectionPanel.SetActive(true);
        foreach (Transform child in EquipmentSelectionPanel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (ItemStack item in PartyManager.instance.inventory)
        {
            if (item.item is Equippable equipItem && equipItem.equipSlot == slot)
            {
                Button btn = Instantiate(EquipmentButtonPrefab, EquipmentSelectionPanel.transform);
                btn.GetComponentInChildren<TextMeshProUGUI>().text = item.item.ItemName + " (" + item.count + ")";
                btn.onClick.AddListener(() => {
                    EquipItemToMember(member, equipItem, slot);
                    EquipmentSelectionPanel.SetActive(false);
                    SetupEquipmentSelectionButtons(member);
                });
            }
        }
    }

    private void EquipItemToMember(PlayerCharData member, Equippable item, EquipSlot slot)
    {
        switch (slot)
        {
            case EquipSlot.Weapon:
                member.weapon = item;
                break;
            case EquipSlot.Head:
                member.head = item;
                break;
            case EquipSlot.Body:
                member.body = item;
                break;
            case EquipSlot.Accessory:
                if (member.accessory1 == null)
                    member.accessory1 = item;
                else
                    member.accessory2 = item;
                break;
        }
    }

    private void ClearCards()
    {
        foreach (Transform child in PartyMembersContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
