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


    private DualSynergyAbility[] masterSynergyList;
    private TriSynergyAbility[] masterTriSynergyList;
    private GameObject activeSubMenu;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        masterSynergyList = Resources.LoadAll<DualSynergyAbility>("Synergies/Dual");
        masterTriSynergyList = Resources.LoadAll<TriSynergyAbility>("Synergies/Tri");
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
            turnOrderElemOther.GetComponentInChildren<TextMeshProUGUI>().text = turnOrder[i].EntityName;
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
                    bool wantsToDualSynergy = !wantsToPrep && Keyboard.current.leftCtrlKey.isPressed;
                    bool wantsToTriSynergy = !wantsToPrep && !wantsToDualSynergy && Keyboard.current.leftAltKey.isPressed;
                    if (wantsToPrep)
                    {
                        pc.StartPrep(new Ability[] {ability});
                        ClearCommandMenu();
                        Destroy(activeSubMenu);
                        BattleManager.instance.NextTurn();
                        return;
                    }

                    if (wantsToDualSynergy)
                    {
                        CharBattle partner;
                        SynergyAbility synergy = BattleUIManager.instance.GetSynergyForAbility(pc, ability, out partner);

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

                    if (wantsToTriSynergy)
                    {
                        foreach (var stance in BattleManager.instance.GetSynergyStances())
                        {
                            Ability b = stance.users[0].GetPreppedAbility();
                            Ability c = stance.users[1].GetPreppedAbility();
                            TriSynergyAbility triple = GetTripleSynergy(ability, b, c);
                            if (triple != null)
                            {
                                pc.StorePreppedAbility(ability);
                                Debug.Log($"TRIPLE SYNERGY FOUND: {triple.Name} with {pc.CharName}, {stance.users[0].CharName}, and {stance.users[1].CharName}!");
                                TargetSelectionManager.instance.BeginTargetSelection(
                                    new CharBattle[] { pc, stance.users[0], stance.users[1] }, triple);
                                
                                ClearCommandMenu();
                                return;
                            }
                        }
                        Debug.Log("No valid triple synergy found for " + ability.Name);
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
                    if (item.targetType is TargetType.DeadAlly or TargetType.DeadAllies && BattleManager.instance.playerChars.All(pc => pc.GetIfAlive()))
                    {
                        return;
                    }
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
                    potentialPartner.transform.position = pc.transform.position + new Vector3(-1, 0, 0);
                    pc.transform.position = pc.transform.position + new Vector3(0.5f, 0, 0);
                });
            }
        });
    }

    public void ShowSynergyCommandMenu(SynergyStance stance)
    {
        CharBattle[] users = stance.users;
        GameObject cm = Instantiate(commandMenuPrefab, commandMenuUIContainer);
        cm.transform.Find("Synergize").gameObject.SetActive(false);

        Button atkBtn = cm.transform.Find("Attack").GetComponent<Button>();
        atkBtn.onClick.AddListener(() =>
            TargetSelectionManager.instance.BeginTargetSelection(users, new SynergyAttack(stance.users)));

        Button defendBtn = cm.transform.Find("Defend").GetComponent<Button>();
        defendBtn.onClick.AddListener(() =>
            TargetSelectionManager.instance.BeginTargetSelection(users, new SynergyDefend(stance.users)));
        
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
                    if (item.targetType is TargetType.DeadAlly or TargetType.DeadAllies && BattleManager.instance.playerChars.All(pc => pc.GetIfAlive()))
                    {
                        return;
                    }
                    TargetSelectionManager.instance.BeginTargetSelection(users, item);
                });
            }
        });

        Button abilityBtn = cm.transform.Find("Ability").GetComponent<Button>();
        abilityBtn.onClick.AddListener(() =>
        {
            ClearSubMenu();
            activeSubMenu = Instantiate(commandSubMenuPrefab, commandMenuUIContainer);
            Vector3 screenPos = cm.GetComponent<RectTransform>().position;
            screenPos.x += 165;
            activeSubMenu.GetComponent<RectTransform>().position = screenPos;

            foreach (var pc in stance.users)
            {
                foreach (var ability in pc.abilities.Skip(2))
                {
                    GameObject btnObj = Instantiate(commandMenuButtonPrefab, activeSubMenu.transform);
                    
                    btnObj.GetComponentInChildren<TextMeshProUGUI>().text = $"{pc.CharName}: {ability.Name}";
                    
                    btnObj.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        TargetSelectionManager.instance.BeginTargetSelection(new CharBattle[] { pc }, ability);
                        ClearCommandMenu();
                    });
                }
            }

            List<DualSynergyAbility> availableSynergies = SynergyStanceManager.instance.GetAvailableSynergiesForStance(stance);
            foreach (var synergy in availableSynergies)
            {
                foreach (var recipe in synergy.synergyTagSets)
                {
                    GenerateButtonsForMatch(stance, recipe, synergy);
                }
            }
        });
    }

    private void GenerateButtonsForMatch(SynergyStance stance, SynergyTagSet recipe, SynergyAbility synergy)
    {
        CharBattle u0 = stance.users[0];
        CharBattle u1 = stance.users[1];

        // Case A: User 0 has Tag 1, User 1 has Tag 2
        var m0_T1 = u0.abilities.Where(a => a.SynergyTags.Contains(recipe.tag1));
        var m1_T2 = u1.abilities.Where(a => a.SynergyTags.Contains(recipe.tag2));
        CreateCombinationButtons(u0, u1, m0_T1, m1_T2, stance, synergy);

        // Case B: User 0 has Tag 2, User 1 has Tag 1 
        // (Only run if Tag1 and Tag2 aren't the same, to avoid duplicates)
        if (recipe.tag1 != recipe.tag2)
        {
            var m0_T2 = u0.abilities.Where(a => a.SynergyTags.Contains(recipe.tag2));
            var m1_T1 = u1.abilities.Where(a => a.SynergyTags.Contains(recipe.tag1));
            CreateCombinationButtons(u0, u1, m0_T2, m1_T1, stance, synergy);
        }
    }

    private void CreateCombinationButtons(CharBattle userA, CharBattle userB, IEnumerable<Ability> movesA, IEnumerable<Ability> movesB, SynergyStance stance, SynergyAbility synergy)
    {
        foreach (var aMove in movesA)
        {
            foreach (var bMove in movesB)
            {
                GameObject btnObj = Instantiate(commandMenuButtonPrefab, activeSubMenu.transform);
                btnObj.GetComponentInChildren<TextMeshProUGUI>().text = $"{synergy.Name}\n<size=70%>({aMove.Name} + {bMove.Name})</size>";

                // Capture local references for the listener
                Ability finalA = aMove;
                Ability finalB = bMove;

                btnObj.GetComponent<Button>().onClick.AddListener(() =>
                {
                    if (Keyboard.current.leftShiftKey.isPressed)
                    {
                        stance.StartPrep(new Ability[] { finalA, finalB });
                        ClearCommandMenu();
                        BattleManager.instance.NextTurn();
                        return;
                    }

                    if (Keyboard.current.leftCtrlKey.isPressed)
                    {
                        foreach (var thirdMember in BattleManager.instance.playerChars.Where(p => 
                                p.GetIfAlive() && !stance.users.Contains(p) && p.GetPreppedAbility() != null))
                        {
                            TriSynergyAbility triple = GetTripleSynergy(finalA, finalB, thirdMember.GetPreppedAbility());
                            if (triple != null)
                            {
                                
                                stance.users[0].StorePreppedAbility(finalA);
                                stance.users[1].StorePreppedAbility(finalB);
                                thirdMember.StorePreppedAbility(thirdMember.GetPreppedAbility());

                                TargetSelectionManager.instance.BeginTargetSelection(
                                    new CharBattle[] { stance.users[0], stance.users[1], thirdMember }, triple);
                                
                                ClearCommandMenu();
                                return;
                            }
                        }
                    } 

                    // Standard Instant Execution
                    userA.StorePreppedAbility(finalA);
                    userB.StorePreppedAbility(finalB);
                    TargetSelectionManager.instance.BeginTargetSelection(new CharBattle[] { userA, userB }, synergy);
                    ClearCommandMenu();
                });
            }
        }
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

    private DualSynergyAbility GetSynergyForAbility(CharBattle currentActor, Ability currentAbility, out CharBattle partner)
    {
        partner = null;

        foreach (var potentialPartner in BattleManager.instance.playerChars)
        {
            if (potentialPartner.GetIfInSynergyStance()) continue;
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

    private TriSynergyAbility GetTripleSynergy(Ability a, Ability b, Ability c)
    {
        if (c == null) return null;
        foreach (var triSynergyAbility in masterTriSynergyList)
        {
            foreach (var recipe in triSynergyAbility.SynergyTagTrios) // You'll need a TripleTagSet with tag1, tag2, tag3
            {
                if (CheckTrioMatch(a, b, c, recipe))
                {
                    return triSynergyAbility;
                }
            }
        }
        return null;
    }

    private bool CheckMatch(Ability a, Ability b, SynergyTagSet recipe)
    {
        bool aMatches1 = a.SynergyTags.Contains(recipe.tag1);
        bool aMatches2 = a.SynergyTags.Contains(recipe.tag2);
        bool bMatches1 = b.SynergyTags.Contains(recipe.tag1);
        bool bMatches2 = b.SynergyTags.Contains(recipe.tag2);

        return (aMatches1 && bMatches2) || (aMatches2 && bMatches1);
    }

    private bool CheckTrioMatch(Ability a, Ability b, Ability c, SynergyTagTrio recipe)
    {
        bool aHasTag1 = a.SynergyTags.Contains(recipe.tag1);
        bool aHasTag2 = a.SynergyTags.Contains(recipe.tag2);
        bool aHasTag3 = a.SynergyTags.Contains(recipe.tag3);

        bool bHasTag1 = b.SynergyTags.Contains(recipe.tag1);
        bool bHasTag2 = b.SynergyTags.Contains(recipe.tag2);
        bool bHasTag3 = b.SynergyTags.Contains(recipe.tag3);

        bool cHasTag1 = c.SynergyTags.Contains(recipe.tag1);
        bool cHasTag2 = c.SynergyTags.Contains(recipe.tag2);
        bool cHasTag3 = c.SynergyTags.Contains(recipe.tag3);

        if (aHasTag1 && bHasTag2 && cHasTag3 || 
            aHasTag1 && bHasTag3 && cHasTag2 || 
            aHasTag2 && bHasTag1 && cHasTag3 || 
            aHasTag2 && bHasTag3 && cHasTag1 || 
            aHasTag3 && bHasTag1 && cHasTag2 || 
            aHasTag3 && bHasTag2 && cHasTag1)
        {
            return true;
        }
        
        return false;
    }
}
