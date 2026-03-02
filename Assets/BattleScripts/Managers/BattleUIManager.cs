using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.Events;
using Unity.VisualScripting;
using System;

public class BattleUIManager : MonoBehaviour
{
    public static BattleUIManager instance;
    public Transform partyUIContainer;
    public Transform commandMenuUIContainer;
    public Transform TurnOrderUIContainer;
    public GameObject CommandMenu;
    public GameObject Submenu;
    public GameObject MenuButtonPrefab;
    public GameObject PopupPrefab;
    public GameObject turnOrderIconPrefab;
    private SynergySearchLogic synergySearchLogic;

    void Awake()
    {
        instance = this;
        synergySearchLogic = new SynergySearchLogic();
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
        HideSubMenu();
        CommandMenu.SetActive(true);
        PositionCommandMenu(entity);

        if (FlowManager.instance.currentFlow >= 20 && !SynergyStanceManager.instance.GetIfStanceExists()) CommandMenu.transform.Find("Synergize").gameObject.SetActive(true);
        else CommandMenu.transform.Find("Synergize").gameObject.SetActive(false);
        
        if (entity is PlayerCharBattle pc)
        {
            BindStaticButton("Attack", () => { TargetSelectionManager.instance.BeginTargetSelection(new CharBattle[] { pc }, pc.abilities[0]); HideCommandMenu(); HideSubMenu(); });
            BindStaticButton("Defend", () => { TargetSelectionManager.instance.BeginTargetSelection(new CharBattle[] { pc }, pc.abilities[1]); HideCommandMenu(); HideSubMenu(); });
            BindStaticButton("Ability", () => CreateAbilityList(pc));
            BindStaticButton("Item", () => CreateItemList(pc));
            BindStaticButton("Synergize", () => CreateDuoList(pc));
        }
        else if (entity is SynergyStance stance)
        {
            BindStaticButton("Attack", () => { TargetSelectionManager.instance.BeginTargetSelection(stance.users, new SynergyAttack(stance.users)); HideCommandMenu(); });
            BindStaticButton("Defend", () => { TargetSelectionManager.instance.BeginTargetSelection(stance.users, new SynergyDefend(stance.users)); HideCommandMenu(); });
            BindStaticButton("Ability", () => CreateSynergyAbilityList(stance));
            BindStaticButton("Item", () => CreateItemList(stance));
        }
    }

    private void PositionCommandMenu(ITurnEntity entity)
    {
        Vector3 screenPos;
        if (entity is CharBattle charEntity) screenPos = Camera.main.WorldToScreenPoint(charEntity.transform.position);
        else if (entity is SynergyStance stance) screenPos = Camera.main.WorldToScreenPoint(stance.users[0].transform.position);
        else screenPos = new Vector3(Screen.width / 2, Screen.height / 2, 0);        
        CommandMenu.GetComponent<RectTransform>().position = new Vector3(screenPos.x + 120, screenPos.y - 50, 0); 
    }

    private void PositionSubMenu()
    {
        Submenu.GetComponent<RectTransform>().position = new Vector3(CommandMenu.GetComponent<RectTransform>().position.x + 165, CommandMenu.GetComponent<RectTransform>().position.y, 0);
    }

    private void BindStaticButton(string buttonName, UnityAction action)
    {
        Button btn = CommandMenu.transform.Find(buttonName).GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(action);
    }

    private void CreateDynamicButton(string abilityName, UnityAction action)
    {
        GameObject btnObj = Instantiate(MenuButtonPrefab, Submenu.transform);
        btnObj.GetComponentInChildren<TextMeshProUGUI>().text = abilityName;
        btnObj.GetComponent<Button>().onClick.AddListener(action);
    }

    private void ClearSubmenuButtons()
    {
        foreach (Transform child in Submenu.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void CreateAbilityList(PlayerCharBattle pc)
    {
        HideSubMenu();
        Submenu.SetActive(true);
        PositionSubMenu();
        for (int i = 2; i < pc.abilities.Count; i++)
        {
            Ability ability = pc.abilities[i];
            CreateDynamicButton(ability.Name, () => HandleAbility(pc, ability));
        }
    }

    private void HandleAbility(PlayerCharBattle pc, Ability ability)
    {
        bool prepping = Keyboard.current.leftShiftKey.isPressed;
        bool seekingDualSynergy = Keyboard.current.leftCtrlKey.isPressed;
        bool seekingTriSynergy = Keyboard.current.leftAltKey.isPressed;

        if (prepping)
        {
            pc.StartPrep(new Ability[] { ability });
            HideCommandMenu();
            HideSubMenu();
            BattleManager.instance.NextTurn();
            return;
        }

        if (seekingDualSynergy)
        {
            CharBattle partner;
            SynergyAbility synergy = synergySearchLogic.GetDoubleSynergy(pc, ability, out partner);

            if (synergy != null)
            {
                pc.StorePreppedAbility(ability);
                TargetSelectionManager.instance.BeginTargetSelection(new CharBattle[] { pc, partner }, synergy);
                HideCommandMenu();
                HideSubMenu();
                return;
            }
            else
            {
                Debug.Log("No valid synergy found for " + ability.Name);
            }
        }

        if (seekingTriSynergy)
        {
            CharBattle[] users = new CharBattle[] { pc };
            List<SynergyStance> potentialStance = BattleManager.instance.GetSynergyStances();
                foreach (var stance in potentialStance)
                {
                    if (stance != null)
                    {
                        Ability preppedAbilityA = stance.users[0].GetPreppedAbility();
                        Ability preppedAbilityB = stance.users[1].GetPreppedAbility();

                        TriSynergyAbility triSynergy = synergySearchLogic.GetTripleSynergy(ability, preppedAbilityA, preppedAbilityB);
                        if (triSynergy != null)
                        {
                            Debug.Log($"{pc.CharName} is performing a triple synergy with {stance.users[0].CharName} and {stance.users[1].CharName}! Synergy: {triSynergy.Name}");
                            pc.StorePreppedAbility(ability);
                            TargetSelectionManager.instance.BeginTargetSelection(users, triSynergy);
                            HideCommandMenu();
                            HideSubMenu();
                            Submenu.SetActive(false);
                            return;
                        }
                        else
                        {
                            Debug.Log("No valid triple synergy found for " + ability.Name);
                        }
                    }
                }
            }
        HideCommandMenu();
        HideSubMenu();
        TargetSelectionManager.instance.BeginTargetSelection(new CharBattle[] { pc }, ability);
    }

    private void CreateSynergyAbilityList(SynergyStance stance)
    {
        HideSubMenu();
        Submenu.SetActive(true);
        PositionSubMenu();
        foreach (PlayerCharBattle user in stance.users)
        {
            for (int i = 2; i < user.abilities.Count; i++)
            {
                Ability ability = user.abilities[i];
                CreateDynamicButton($"{user.CharName}: {ability.Name}", () => HandleAbility(user, ability));
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
    }

    private void CreateItemList(ITurnEntity entity)
    {
        HideSubMenu();
        Submenu.SetActive(true);
        PositionSubMenu();
        foreach (var pair in InventoryManager.instance.items)
        {
            Item item = pair.Key;
            int count = pair.Value;

            CreateDynamicButton($"{item.Name} (x{count})", () =>
            {
                if (item.targetType is TargetType.DeadAlly or TargetType.DeadAllies && BattleManager.instance.playerChars.All(pc => pc.GetIfAlive()))
                {
                    return;
                }
                TargetSelectionManager.instance.BeginTargetSelection(new CharBattle[] { entity as CharBattle }, item);
            });
        }
    }

    private void CreateDuoList(PlayerCharBattle pc)
    {
        HideSubMenu();
        Submenu.SetActive(true);
        PositionSubMenu();
        foreach (var player in BattleManager.instance.playerChars.Where(p => p.GetIfAlive() && p != pc)){
            PlayerCharBattle potentialPartner = player;
            CreateDynamicButton(player.CharName, () =>
            {
                Debug.Log($"{pc.CharName} is synergizing with {potentialPartner.CharName}!");
                SynergyStanceManager.instance.CreateSynergyStance(new CharBattle[] { pc, potentialPartner });
                potentialPartner.transform.position = pc.transform.position + new Vector3(-1, 0, 0);
                pc.transform.position = pc.transform.position + new Vector3(0.5f, 0, 0);
            });
        }
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
                GameObject btnObj = Instantiate(MenuButtonPrefab, Submenu.transform);
                btnObj.GetComponentInChildren<TextMeshProUGUI>().text = $"{synergy.Name}\n<size=70%>({aMove.Name} + {bMove.Name})</size>";

                // Capture local references for the listener
                Ability finalA = aMove;
                Ability finalB = bMove;

                btnObj.GetComponent<Button>().onClick.AddListener(() =>
                {
                    if (Keyboard.current.leftShiftKey.isPressed)
                    {
                        stance.StartPrep(new Ability[] { finalA, finalB });
                        HideCommandMenu();
                        BattleManager.instance.NextTurn();
                        return;
                    }

                    if (Keyboard.current.leftCtrlKey.isPressed)
                    {
                        foreach (var thirdMember in BattleManager.instance.playerChars.Where(p => 
                                p.GetIfAlive() && !stance.users.Contains(p) && p.GetPreppedAbility() != null))
                        {
                            TriSynergyAbility triple = synergySearchLogic.GetTripleSynergy(finalA, finalB, thirdMember.GetPreppedAbility());
                            if (triple != null)
                            {
                                
                                stance.users[0].StorePreppedAbility(finalA);
                                stance.users[1].StorePreppedAbility(finalB);
                                thirdMember.StorePreppedAbility(thirdMember.GetPreppedAbility());

                                TargetSelectionManager.instance.BeginTargetSelection(
                                    new CharBattle[] { stance.users[0], stance.users[1], thirdMember }, triple);
                                
                                HideCommandMenu();
                                HideSubMenu();
                                return;
                            }
                        }
                    } 

                    // Standard Instant Execution
                    userA.StorePreppedAbility(finalA);
                    userB.StorePreppedAbility(finalB);
                    TargetSelectionManager.instance.BeginTargetSelection(new CharBattle[] { userA, userB }, synergy);
                    HideCommandMenu();
                });
            }
        }
    }

    public void HideCommandMenu()
    {
        CommandMenu.SetActive(false);
    }

    public void HideSubMenu()
    {
        Submenu.SetActive(false);
        ClearSubmenuButtons();
    }

    public void Popup(int damage, Vector3 position, PopupType type)
    {
        GameObject popup = Instantiate(PopupPrefab, position + new Vector3(0, 1.5f, 0), Quaternion.identity);
        Popup popupScript = popup.GetComponent<Popup>();
        popupScript.Setup(damage, position, type);
    }
}
