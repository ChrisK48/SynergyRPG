using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.Events;
using System;

public class BattleUIManager : MonoBehaviour
{
    public static BattleUIManager instance;
    public Transform partyUIContainer;
    public Transform commandMenuUIContainer;
    public Transform TurnOrderUIContainer;
    public Transform BattleEndContainer;
    public Transform FlowUIContainer;
    public GameObject ExpUIPrefab;
    public GameObject CommandMenu;
    public GameObject Submenu;
    public GameObject MenuButtonPrefab;
    public GameObject FastTrackButton;
    public RectTransform ButtonContainer;
    public RectTransform FastTrackButtonContainer;
    public GameObject PopupPrefab;
    public GameObject turnOrderIconPrefab;

    public Button EndBattleButton;

    private SynergySearchLogic synergySearchLogic;
    private bool fastTrack = false;

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

        Button synergyButton = CommandMenu.transform.Find("Synergize").GetComponent<Button>();

        if (FlowManager.instance.currentFlow >= 20 && !SynergyStanceManager.instance.GetIfStanceExists()) 
        {
            synergyButton.gameObject.SetActive(true);
            synergyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Synergize";
        }
        else if (entity is SynergyStance stance) 
        {
            synergyButton.gameObject.SetActive(true);
            synergyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Cancel Synergy";
        }
        else synergyButton.gameObject.SetActive(false);
        
        if (entity is PlayerCharBattle pc)
        {
            BindStaticButton("Attack", () => { TargetSelectionManager.instance.BeginTargetSelection(new CharBattle[] { pc }, pc.abilities[0]); HideCommandMenu(); HideSubMenu(); });
            BindStaticButton("Defend", () => { pc.Defend(); HideCommandMenu(); HideSubMenu(); pc.EndTurn(); });
            BindStaticButton("Ability", () => CreateAbilityList(pc));
            BindStaticButton("Item", () => CreateItemList(pc));
            BindStaticButton("Synergize", () => CreateDuoList(pc));
        }
        else if (entity is SynergyStance stance)
        {
            BindStaticButton("Attack", () => { TargetSelectionManager.instance.BeginTargetSelection(stance.users, new SynergyAttack(stance.users)); HideCommandMenu(); });
            BindStaticButton("Defend", () => { stance.Defend(); HideCommandMenu(); HideSubMenu(); stance.EndTurn(); });
            BindStaticButton("Ability", () => CreateSynergyAbilityList(stance));
            BindStaticButton("Item", () => CreateItemList(stance));
            BindStaticButton("Synergize", () => {
                HideCommandMenu();
                SynergyStanceManager.instance.CancelSynergyStance(stance);
            });
        }
    }

    private void PositionCommandMenu(ITurnEntity entity)
    {
        Vector3 screenPos;
        if (entity is CharBattle charEntity) screenPos = Camera.main.WorldToScreenPoint(charEntity.transform.position);
        else if (entity is SynergyStance stance) screenPos = Camera.main.WorldToScreenPoint(stance.users[0].transform.position);
        else screenPos = new Vector3(Screen.width / 2, Screen.height / 2, 0);        
        CommandMenu.GetComponent<RectTransform>().position = new Vector3(screenPos.x + 120, screenPos.y + 50, 0); 
    }

    private void PositionSubMenu()
    {
        Submenu.GetComponent<RectTransform>().position = new Vector3(CommandMenu.GetComponent<RectTransform>().position.x + 300, CommandMenu.GetComponent<RectTransform>().position.y, 0);
    }

    private void BindStaticButton(string buttonName, UnityAction action)
    {
        Button btn = CommandMenu.transform.Find(buttonName).GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(action);
    }

    private void CreateDynamicButton(string abilityName, UnityAction action, Transform parent)
    {
        GameObject btnObj = Instantiate(MenuButtonPrefab, parent);
        btnObj.GetComponentInChildren<TextMeshProUGUI>().text = abilityName;
        btnObj.GetComponent<Button>().onClick.AddListener(action);
    }

    private void ClearSubmenuButtons()
    {
        FastTrackButton.SetActive(false);
        foreach (Transform child in ButtonContainer)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in FastTrackButtonContainer)
        {
            Destroy(child.gameObject);
        }
        ButtonContainer.gameObject.SetActive(true);
        FastTrackButtonContainer.gameObject.SetActive(false);
    }

    private void CreateAbilityList(PlayerCharBattle pc)
    {
        HideSubMenu();
        Submenu.SetActive(true);
        FastTrackButton.SetActive(true);
        
        for (int i = 1; i < pc.abilities.Count; i++)
        {
            Ability ability = pc.abilities[i];
            CreateDynamicButton(ability.Name, () => {
                if (pc.CanPerformAbility(ability)) HandleAbility(pc, ability);
            }, ButtonContainer);
        }
        CreateButtonsForSynergies(pc);


        Button btn = FastTrackButton.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => {
            fastTrack = !fastTrack;
            
            ButtonContainer.gameObject.SetActive(!fastTrack);
            FastTrackButtonContainer.gameObject.SetActive(fastTrack);
            if (fastTrack && FastTrackButtonContainer.childCount == 0) CreateFastTrackList(pc);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = fastTrack ? "Standard" : "FastTrack";
        });

        PositionSubMenu();
    }

    private void CreateFastTrackList(PlayerCharBattle pc)
    {
        foreach (var ability in pc.abilities)
        {
            List<Tuple<DualSynergyAbility, CharBattle>> synergies = synergySearchLogic.GetPotentialPairings(pc, ability);
            foreach (var synergy in synergies)
            {
                CreateDynamicButton($"{synergy.Item1.Name} with {synergy.Item2.CharName}", () =>
                {
                    if (FlowManager.instance.currentFlow < 10) return;
                    FlowManager.instance.ConsumeFlow(10);
                    pc.StorePreppedAbility(ability);
                    TargetSelectionManager.instance.BeginTargetSelection(new CharBattle[] { pc }, synergy.Item1);
                    HideCommandMenu();
                    HideSubMenu();
                    FlowManager.instance.ConsumeFlow(10);
                }, FastTrackButtonContainer);
            }
        }
    }

    private void HandleAbility(PlayerCharBattle pc, Ability ability)
    {
        bool prepping = Keyboard.current.leftShiftKey.isPressed;

        if (prepping)
        {
            pc.StartPrep(new Ability[] { ability });
            HideCommandMenu();
            HideSubMenu();
            pc.EndTurn();
            return;
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
                CreateDynamicButton($"{user.CharName}: {ability.Name}", () => HandleAbility(user, ability), ButtonContainer);
            }
        }

        List<DualSynergyAbility> availableSynergies = SynergyStanceManager.instance.GetAvailableSynergiesForStance(stance);
        foreach (var synergy in availableSynergies)
        {
            foreach (var recipe in synergy.SynergyTagSets)
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
            ConsumableItem item = pair.Key;
            int count = pair.Value;

            CreateDynamicButton($"{item.Name} (x{count})", () =>
            {
                if (item.targetType is TargetType.DeadAlly or TargetType.DeadAllies && BattleManager.instance.playerChars.All(pc => pc.GetIfAlive()))
                {
                    return;
                }
                TargetSelectionManager.instance.BeginTargetSelection(new CharBattle[] { entity as CharBattle }, item);
            }, ButtonContainer);
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
            }, ButtonContainer);
        }
    }

    
    private void GenerateButtonsForMatch(SynergyStance stance, SynergyTagSet recipe, SynergyAbility synergy)
    {
        if(stance.users[0] is PlayerCharBattle pc0 && stance.users[1] is PlayerCharBattle pc1)
        {
            // Case A: User 0 has Tag 1, User 1 has Tag 2
            var m0_T1 = pc0.abilities.Where(a => a.SynergyTags.Contains(recipe.tag1));
            var m1_T2 = pc1.abilities.Where(a => a.SynergyTags.Contains(recipe.tag2));
            CreateCombinationButtons(pc0, pc1, m0_T1, m1_T2, stance, synergy);

            // Case B: User 0 has Tag 2, User 1 has Tag 1 
            // (Only run if Tag1 and Tag2 aren't the same, to avoid duplicates)
            if (recipe.tag1 != recipe.tag2)
            {
                var m0_T2 = pc0.abilities.Where(a => a.SynergyTags.Contains(recipe.tag2));
                var m1_T1 = pc1.abilities.Where(a => a.SynergyTags.Contains(recipe.tag1));
                CreateCombinationButtons(pc0, pc1, m0_T2, m1_T1, stance, synergy);
            }
        }
    }

    private void CreateButtonsForSynergies(PlayerCharBattle user)
    {
        foreach (Ability ability in user.abilities)
        {
            List<Tuple<DualSynergyAbility, CharBattle>> synergies = synergySearchLogic.GetDoubleSynergy(user, ability);
            if (synergies.Count > 0)
            {
                foreach (var synergy in synergies)
                {
                    PlayerCharBattle finalPartner1 = (PlayerCharBattle)synergy.Item2;
                    SynergyAbility finalAbility = synergy.Item1;
                    CreateDynamicButton($"{synergy.Item1.Name} with {synergy.Item2.CharName}", () =>
                    {
                        if (finalPartner1.CanPerformAbility(finalPartner1.GetPreppedAbility()) && user.CanPerformAbility(ability))
                        {
                            user.StorePreppedAbility(ability);
                            TargetSelectionManager.instance.BeginTargetSelection(new CharBattle[] { user, finalPartner1 }, finalAbility);
                            HideCommandMenu();
                            HideSubMenu();
                        }
                    }, ButtonContainer);
                }
            }

            List<Tuple<TriSynergyAbility, CharBattle[]>> triSynergies = synergySearchLogic.GetTripleSynergy(user, ability);
            if (triSynergies.Count > 0)            
            {
                PlayerCharBattle finalPartner1 = (PlayerCharBattle)triSynergies[0].Item2[0];
                PlayerCharBattle finalPartner2 = (PlayerCharBattle)triSynergies[0].Item2[1];
                SynergyAbility finalTriSynergy = triSynergies[0].Item1;
                CreateDynamicButton($"{triSynergies[0].Item1.Name} with {triSynergies[0].Item2[0].CharName} and {triSynergies[0].Item2[1].CharName}", () =>
                {
                    if (finalPartner1.CanPerformAbility(user.GetPreppedAbility()) && finalPartner2.CanPerformAbility(user.GetPreppedAbility()) && user.CanPerformAbility(ability))
                    {
                        user.StorePreppedAbility(ability);
                        TargetSelectionManager.instance.BeginTargetSelection(new CharBattle[] { user, finalPartner1, finalPartner2 }, finalTriSynergy);
                        HideCommandMenu();
                        HideSubMenu();
                    }
                }, ButtonContainer);
            }
        }
    }

    private void CreateCombinationButtons(CharBattle userA, CharBattle userB, IEnumerable<Ability> movesA, IEnumerable<Ability> movesB, SynergyStance stance, SynergyAbility synergy)
    {
        foreach (var aMove in movesA)
        {
            foreach (var bMove in movesB)
            {
                GameObject btnObj = Instantiate(MenuButtonPrefab, ButtonContainer);
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

                    // Standard Instant Execution
                    userA.StorePreppedAbility(finalA);
                    userB.StorePreppedAbility(finalB);
                    TargetSelectionManager.instance.BeginTargetSelection(new CharBattle[] { userA, userB }, synergy);
                    HideCommandMenu();
                    HideSubMenu();
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

    public bool GetIfFastTracked() => fastTrack;

    public void ShowVictoryScreen(int earnedXp, int earnedMoney)
    {
        foreach (PlayerCharData pc in PartyManager.instance.activePartyMembers)
        {
            int oldExp = pc.currentExp;
            pc.GainExp(earnedXp);
            GameObject obj = Instantiate(ExpUIPrefab, BattleEndContainer);
            ExpUI charExp = obj.GetComponent<ExpUI>();
            Debug.Log($"Earned Exp: {earnedXp}, Current Exp: {pc.currentExp}, Old Exp: {oldExp}");
            charExp.SetupUI(pc, oldExp, BattleManager.instance.GetEarnedXp());
        }

        PartyManager.instance.heldMoney += earnedMoney;

        partyUIContainer.gameObject.SetActive(false);
        TurnOrderUIContainer.gameObject.SetActive(false);
        FlowUIContainer.gameObject.SetActive(false);

        EndBattleButton.onClick.RemoveAllListeners();
        EndBattleButton.onClick.AddListener(() => {
            BattleTransitionManager.instance.ExitBattle();
        });
        EndBattleButton.gameObject.SetActive(true);
    }
}
