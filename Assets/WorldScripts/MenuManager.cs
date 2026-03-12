using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    public GameObject MenuPanel;
    public GameObject PartyMemberCardPrefab;
    private bool menuOpen = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        BuildMenu();
    }

    public void ToggleMenu()
    {
        menuOpen = !menuOpen;
        MenuPanel.SetActive(menuOpen);
    }

    public bool GetIfMenuOpen() { return menuOpen; }

    private void BuildMenu()
    {
        foreach (var member in PartyManager.instance.activePartyMembers)
        {
            GameObject card = Instantiate(PartyMemberCardPrefab, MenuPanel.transform);
            card.GetComponent<PartyMemberCard>().Initialize(member);
        }
    }
}
