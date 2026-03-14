using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    [Header("Main Menu")]
    public GameObject MenuPanel;
    public Button StatsButton;
    public Button InventoryButton;
    public Button EquipmentButton;
    public GameObject PartyMemberCardContainer;
    public GameObject PartyMemberStatsPrefab;
    public GameObject PartyMemberCardPrefab;


    [Header("Inventory Menu")]
    public GameObject InventoryContainer;
    public GameObject InventoryCardContainer;
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
    private bool menuOpen = false;
    private bool inventoryOpen = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        BuildActiveMemberCards();
        StatsButton.onClick.AddListener(BuildMemberStatCards);
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
            GameObject card = Instantiate(PartyMemberCardPrefab, PartyMemberCardContainer.transform);
            card.GetComponent<PartyMemberCard>().Initialize(member);
        }
    }

    private void BuildMemberStatCards()
    {
        ClearCards();
        foreach (var member in Resources.LoadAll<PlayerCharData>("Data/PartyStats"))
        {
            GameObject card = Instantiate(PartyMemberStatsPrefab, PartyMemberCardContainer.transform);
            card.GetComponent<PartyMemberStatsCard>().Initialize(member);
        }
    }

    private void BuildInventoryMenu()
    {
        foreach (Transform child in InventoryCardContainer.transform)
        {
            Destroy(child.gameObject);
        }

        InventoryContainer.SetActive(true);
        inventoryOpen = true;
        foreach(ItemStack item in PartyManager.instance.inventory)
        {
            GameObject itemCard = Instantiate(ItemCardPrefab, InventoryCardContainer.transform);
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
                GetEquipmentSelectionForChar(selectedMember);
            });
        }
    }

    private void GetEquipmentSelectionForChar(PlayerCharData member)
    {
        WeaponSelectButton.GetComponentInChildren<TextMeshProUGUI>().text = member.weapon?.ItemName ?? "None";
        HelmetSelectButton.GetComponentInChildren<TextMeshProUGUI>().text = member.head?.ItemName ?? "None";
        BodySelectButton.GetComponentInChildren<TextMeshProUGUI>().text = member.body?.ItemName ?? "None";
        Accessory1SelectButton.GetComponentInChildren<TextMeshProUGUI>().text = member.accessory1?.ItemName ?? "None";
        Accessory2SelectButton.GetComponentInChildren<TextMeshProUGUI>().text = member.accessory2?.ItemName ?? "None";
    }

    private void ClearCards()
    {
        foreach (Transform child in PartyMemberCardContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
