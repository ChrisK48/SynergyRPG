using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;
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
    }

    private void ClearCards()
    {
        foreach (Transform child in PartyMembersContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
