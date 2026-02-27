using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Linq;

public class BattleUIManager : MonoBehaviour
{
    public static BattleUIManager instance;
    public Transform partyUIContainer;
    public Transform commandMenuUIContainer;
    public Transform TurnOrderUIContainer;
    public GameObject commandMenuPrefab;
    public GameObject commandSubMenuPrefab;
    public GameObject commandMenuButtonPrefab;
    public GameObject turnOrderIconPrefab;


    private TwoMemberSynergyAbility[] masterSynergyList;
    private GameObject activeSubMenu;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        masterSynergyList = Resources.LoadAll<TwoMemberSynergyAbility>("Synergies");
    }

    public void GeneratePartyUI(List<PlayerCharBattle> playerChars)
    {
        foreach (PlayerCharBattle pc in playerChars)
        {
            GameObject uiElem = Instantiate(pc.uniqueUIPrefab, partyUIContainer);
            PartyUIParent slotScript = uiElem.GetComponent<PartyUIParent>();
            slotScript.SetUpUI(pc);
        }
    }

    public void UpdateTurnOrderUI(List<ITurnEntity> turnOrder, ITurnEntity currentChar, int currentTurnIndex)
    {
        foreach (Transform child in TurnOrderUIContainer)
        {
            Destroy(child.gameObject);
        }

        for(int i = currentTurnIndex; i < turnOrder.Count; i++)
        {
            GameObject turnOrderElemOther = Instantiate(turnOrderIconPrefab, TurnOrderUIContainer);
            turnOrderElemOther.GetComponentInChildren<TextMeshProUGUI>().text = turnOrder[i].entityName;
        }
    }

    public void ShowCommandMenu(ITurnEntity entity)
    {
        if (entity is PlayerCharBattle pc)
        {
            ShowStandardCommandMenu(pc);
        } else if (entity is SynergyStance stance)
        {
            ShowSynergyCommandMenu(stance);
        }
    }

    public void ShowStandardCommandMenu(PlayerCharBattle pc)
    {
        CharBattle[] users = new CharBattle[] { pc };
        GameObject cm = Instantiate(commandMenuPrefab, commandMenuUIContainer);

        // Position the command menu near the character (this needs to be changed but works for now)
        Vector3 screenPos = Camera.main.WorldToScreenPoint(pc.transform.position);
        screenPos.x += 120;
        screenPos.y -= 50;
        cm.GetComponent<RectTransform>().position = screenPos;

        Button attackBtn = cm.transform.Find("Attack").GetComponent<Button>();
        attackBtn.onClick.AddListener(() => 
            TargetSelectionManager.instance.BeginTargetSelection(users, pc.abilities[0]));

        Button defendBtn = cm.transform.Find("Defend").GetComponent<Button>();
        defendBtn.onClick.AddListener(() => 
            TargetSelectionManager.instance.BeginTargetSelection(users, pc.abilities[1]));

        Button abilityBtn = cm.transform.Find("Ability").GetComponent<Button>();
        abilityBtn.onClick.AddListener(() =>
        {
            ClearSubMenu();
            activeSubMenu = Instantiate(commandSubMenuPrefab, commandMenuUIContainer);
            Vector3 screenPos = cm.GetComponent<RectTransform>().position;
            screenPos.x += 165;
            activeSubMenu.GetComponent<RectTransform>().position = screenPos;
            for (int i = 2; i < pc.abilities.Count; i++)
            {
                GameObject abilityBtnSub = Instantiate(commandMenuButtonPrefab, activeSubMenu.transform);
                Ability ability = pc.abilities[i];
                abilityBtnSub.GetComponentInChildren<TextMeshProUGUI>().text = ability.Name;
                abilityBtnSub.GetComponent<Button>().onClick.AddListener(() =>
                {
                    bool wantsToPrep = Keyboard.current.leftShiftKey.isPressed;
                    bool wantsToSynergy = !wantsToPrep && Keyboard.current.leftCtrlKey.isPressed;
                    if (wantsToPrep)
                    {
                        pc.StartPrep(new Ability[] {ability});
                        ClearCommandMenu();
                        Destroy(activeSubMenu);
                        BattleManager.instance.NextTurn();
                        return;
                    }

                    if (wantsToSynergy)
                    {
                        CharBattle partner;
                        TwoMemberSynergyAbility synergy = BattleUIManager.instance.GetSynergyForAbility(pc, ability, out partner);

                        if (synergy != null)
                        {
                            pc.StorePreppedAbility(ability);
                            TargetSelectionManager.instance.BeginTargetSelection(new CharBattle[] {pc, partner}, synergy);
                            ClearCommandMenu();
                            Destroy(activeSubMenu);
                            return;
                        }
                        else
                        {
                            Debug.Log("No valid synergy found for " + ability.Name);
                        }
                    }
                    TargetSelectionManager.instance.BeginTargetSelection(users, ability);
                    Destroy(activeSubMenu);
                });
            }
        });

        Button itemBtn = cm.transform.Find("Item").GetComponent<Button>();
        itemBtn.onClick.AddListener(() =>
        {
            ClearSubMenu();
            activeSubMenu = Instantiate(commandSubMenuPrefab, commandMenuUIContainer);
            Vector3 screenPos = cm.GetComponent<RectTransform>().position;
            screenPos.x += 165;
            activeSubMenu.GetComponent<RectTransform>().position = screenPos;
            foreach (var pair in InventoryManager.instance.items)
            {
                Item item = pair.Key;
                int count = pair.Value;

                GameObject itemBtnObj = Instantiate(commandMenuButtonPrefab, activeSubMenu.transform);
                // Show name and quantity: "Potion (x5)"
                itemBtnObj.GetComponentInChildren<TextMeshProUGUI>().text = $"{item.Name} (x{count})";

                itemBtnObj.GetComponent<Button>().onClick.AddListener(() =>
                {
                    // The Target Manager handles Items and Abilities exactly the same!
                    TargetSelectionManager.instance.BeginTargetSelection(users, item);
                });
            }
        });

        Button synergyBtn = cm.transform.Find("Synergize").GetComponent<Button>();
        synergyBtn.onClick.AddListener(() =>
        {
            ClearSubMenu();
            activeSubMenu = Instantiate(commandSubMenuPrefab, commandMenuUIContainer);
            Vector3 screenPos = cm.GetComponent<RectTransform>().position;
            screenPos.x += 165;
            activeSubMenu.GetComponent<RectTransform>().position = screenPos;
            foreach (var player in BattleManager.instance.playerChars.Where(p => p.GetIfAlive() && p != pc)){
                PlayerCharBattle potentialPartner = player;
                GameObject btnObj = Instantiate(commandMenuButtonPrefab, activeSubMenu.transform);
                btnObj.GetComponentInChildren<TextMeshProUGUI>().text = player.CharName;
                btnObj.GetComponent<Button>().onClick.AddListener(() =>
                {
                    Debug.Log($"{pc.CharName} is synergizing with {potentialPartner.CharName}!");
                    SynergyStanceManager.instance.CreateSynergyStance(new CharBattle[] { pc, potentialPartner });
                });
            }
        });
    }

    public void ShowSynergyCommandMenu(SynergyStance stance)
    {
        CharBattle[] users = stance.users;
        GameObject cm = Instantiate(commandMenuPrefab, commandMenuUIContainer);
        cm.transform.Find("Synergize").gameObject.SetActive(false);

        Button defendBtn = cm.transform.Find("Defend").GetComponent<Button>();
        defendBtn.onClick.AddListener(() =>
            TargetSelectionManager.instance.BeginTargetSelection(users, new SynergyDefend(stance.users)));
    }

    public void ClearCommandMenu()
    {
        foreach (Transform child in commandMenuUIContainer)
        {
            Destroy(child.gameObject);
        }
    }

    public void ClearSubMenu()
    {
        Destroy(activeSubMenu);
    }

    private TwoMemberSynergyAbility GetSynergyForAbility(CharBattle currentActor, Ability currentAbility, out CharBattle partner)
    {
        partner = null;

        foreach (var potentialPartner in BattleManager.instance.playerChars)
        {
            Ability prepped = potentialPartner.GetPreppedAbility();
            
            // Don't combo with yourself!
            if (prepped != null && potentialPartner != currentActor)
            {
                foreach (var synergy in masterSynergyList)
                {
                    // Look through every recipe defined in this specific Synergy Asset
                    foreach (var recipe in synergy.synergyTagSets)
                    {
                        // Because both abilities have Lists of tags, we check if they satisfy the recipe
                        if (CheckMatch(currentAbility, prepped, recipe))
                        {
                            partner = potentialPartner;
                            return synergy;
                        }
                    }
                }
            }
        }
        return null;
    }

    // Helper to check if two abilities satisfy a specific TagSet recipe
    private bool CheckMatch(Ability a, Ability b, SynergyTagSet recipe)
    {
        bool aMatches1 = a.SynergyTags.Contains(recipe.tag1);
        bool aMatches2 = a.SynergyTags.Contains(recipe.tag2);
        bool bMatches1 = b.SynergyTags.Contains(recipe.tag1);
        bool bMatches2 = b.SynergyTags.Contains(recipe.tag2);

        // Case 1: Ability A has Tag1 and Ability B has Tag2
        // Case 2: Ability A has Tag2 and Ability B has Tag1
        return (aMatches1 && bMatches2) || (aMatches2 && bMatches1);
    }
}
