using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.Events;
using System;
using UnityEditor.SettingsManagement;
using System.Runtime.InteropServices;

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
    public GameObject SynergyButton;
    public RectTransform ButtonContainer;
    public RectTransform SynergyButtonContainer;
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

        Button btn = Submenu.transform.Find("SynergyButton").GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => {
            ButtonContainer.gameObject.SetActive(ButtonContainer.gameObject.activeSelf == false);
            SynergyButtonContainer.gameObject.SetActive(ButtonContainer.gameObject.activeSelf == false);
        });
        
        if (entity is PlayerCharBattle pc)
        {
            BindStaticButton("Attack", () => { TargetSelectionManager.instance.BeginTargetSelection(new CharBattle[] { pc }, pc.attackAbility); HideCommandMenu(); HideSubMenu(); });
            BindStaticButton("Defend", () => { pc.Defend(); HideCommandMenu(); HideSubMenu(); pc.EndTurn(); });
            BindStaticButton("Ability", () => CreateAbilityList(pc));
            BindStaticButton("Item", () => CreateItemList(pc));
            BindStaticButton("Synergize", () => CreateStanceList(pc));
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
        Submenu.GetComponent<RectTransform>().position = new Vector3(CommandMenu.GetComponent<RectTransform>().position.x + 500, CommandMenu.GetComponent<RectTransform>().position.y, -100);
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
        SynergyButton.SetActive(false);
        foreach (Transform child in ButtonContainer)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in SynergyButtonContainer)
        {
            Destroy(child.gameObject);
        }
        ButtonContainer.gameObject.SetActive(true);
        SynergyButtonContainer.gameObject.SetActive(false);
    }

    private void CreateAbilityList(PlayerCharBattle pc)
    {
        HideSubMenu();
        Submenu.SetActive(true);
        SynergyButton.SetActive(true);
        
        for (int i = 0; i < pc.abilities.Count; i++)
        {
            Ability ability = pc.abilities[i];
            CreateDynamicButton(ability.Name, () => {
                if (pc.CanPerformAbility(ability)) HandleAbility(pc, ability);
            }, ButtonContainer);
        }
        CreateButtonsForSynergies(pc);
        CreateButtonsForTriSynergies(pc, null);
        PositionSubMenu();
    }

    private void HandleAbility(PlayerCharBattle pc, Ability ability)
    {
        bool prepping = Keyboard.current.leftShiftKey.isPressed;

        if (prepping)
        {
            pc.StartPrep(ability.SynergyTags);
            HideCommandMenu();
            HideSubMenu();
            pc.EndTurn();
            return;
        }

        HideCommandMenu();
        HideSubMenu();
        TargetSelectionManager.instance.BeginTargetSelection(new CharBattle[] { pc }, ability);
    }

    private void HandleDualAbility(PlayerCharBattle userA, PlayerCharBattle userB, DualSynergyAbility ability)
    {
        bool prepping = Keyboard.current.leftShiftKey.isPressed;

        if (prepping) 
        {
            var results = PartyManager.instance.GetAvailableDualSynergies(userA, userB);
            var synergy = results.Find(s => s.Ability == ability);

            if (synergy.Ability != null)
            {
                userA.StartPrep(new List<SynergyTag> { synergy.UserATag });
                userB.StartPrep(new List<SynergyTag> { synergy.UserBTag });
            }

            HideCommandMenu();
            HideSubMenu();

            userA.EndTurn();
            userB.EndTurn();
            return;
        }

        HideCommandMenu();
        HideSubMenu();
        TargetSelectionManager.instance.BeginTargetSelection(new CharBattle[] { userA, userB }, ability);
    }

    private void CreateSynergyAbilityList(SynergyStance stance)
    {
        HideSubMenu();
        Submenu.SetActive(true);
        PositionSubMenu();
        foreach (PlayerCharBattle user in stance.users)
        {
            foreach (Ability ability in user.abilities)
            {
                CreateDynamicButton($"{user.CharName}: {ability.Name}", () => HandleAbility(user, ability), ButtonContainer);
            }
        }
        foreach (DualSynergyResult DualSynergy in PartyManager.instance.GetAvailableDualSynergies((PlayerCharBattle)stance.users[0], (PlayerCharBattle)stance.users[1]))
        {
            CreateDynamicButton($"Synergy: {DualSynergy.Ability.Name}", () => {
                HandleDualAbility((PlayerCharBattle)stance.users[0], (PlayerCharBattle)stance.users[1], DualSynergy.Ability);
            }, ButtonContainer);
        }

        CreateButtonsForTriSynergies((PlayerCharBattle)stance.users[0], (PlayerCharBattle)stance.users[1]);
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

    private void CreateStanceList(PlayerCharBattle pc)
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

    private void CreateButtonsForSynergies(PlayerCharBattle user)
    {
        foreach (DualSynergyResult synergy in PartyManager.instance.GetAvailableDualSynergies(user, null))
        {
            // 1. Correctly identify the partner
            bool isUserA = synergy.UserA.name == user.charData.name;
            PlayerCharData partnerData = isUserA ? synergy.UserB : synergy.UserA;
            PlayerCharBattle partnerBattle = BattleManager.instance.playerChars.Find(pc => pc.charData.name == partnerData.name);

            if (partnerBattle == null) continue;

            SynergyTag partnerReqTag = isUserA ? synergy.UserBTag : synergy.UserATag;
            SynergyTag userReqTag = isUserA ? synergy.UserATag : synergy.UserBTag;

            bool partnerIsReady = partnerBattle.IsPreppingSynergy() && 
                                partnerBattle.GetStoredTags().Any(t => t == partnerReqTag);

            Debug.Log($"[CHECK] User: {user.CharName} | Partner: {partnerBattle.CharName} | Partner Needs: {partnerReqTag} | Partner Has: {(partnerBattle.GetStoredTags().Count > 0 ? partnerBattle.GetStoredTags()[0] : "None")}");

            if (partnerIsReady)
            {
                CreateDynamicButton($"Execute Synergy: {synergy.Ability.Name}", () => {

                    user.StartPrep(new List<SynergyTag> { userReqTag });

                    TargetSelectionManager.instance.BeginTargetSelection(
                        new CharBattle[] { user, partnerBattle }, 
                        synergy.Ability
                    );
                }, SynergyButtonContainer);
            }
        }
    }

    private void CreateButtonsForTriSynergies(PlayerCharBattle userA, PlayerCharBattle userB)
    {
        // Pass userA, userB, and null to find all potential Tri-Synergies involving these players
        foreach (TriSynergyResult synergy in PartyManager.instance.GetAvailableTriSynergies(userA, userB, null))
        {
            List<PlayerCharData> requiredParticipants = new List<PlayerCharData> { synergy.UserA, synergy.UserB, synergy.UserC };
            
            // Remove the current actors from the "Required Partners" check
            requiredParticipants.RemoveAll(p => p.name == userA.charData.name);
            if (userB != null) 
                requiredParticipants.RemoveAll(p => p.name == userB.charData.name);

            bool allPartnersReady = true;
            List<PlayerCharBattle> partnersToJoin = new List<PlayerCharBattle>();

            foreach (PlayerCharData pData in requiredParticipants)
            {
                PlayerCharBattle pBattle = BattleManager.instance.playerChars.Find(pc => pc.charData.name == pData.name);
                
                SynergyTag neededTag;
                if (pData.name == synergy.UserA.name)      neededTag = synergy.UserATag;
                else if (pData.name == synergy.UserB.name) neededTag = synergy.UserBTag;
                else                                      neededTag = synergy.UserCTag;

                // Check if the third/other party member is already prepping the right tag
                if (pBattle != null && pBattle.IsPreppingSynergy() && pBattle.GetStoredTags().Any(t => t == neededTag))
                {
                    partnersToJoin.Add(pBattle);
                }
                else
                {
                    allPartnersReady = false;
                    break;
                }
            }

            if (allPartnersReady)
            {
                // Use SynergyButtonContainer so it stays in the toggled Synergy list
                CreateDynamicButton($"TRI: {synergy.Ability.Name}", () => {
                    StoreTagForUser(userA, synergy);
                    if (userB != null) StoreTagForUser(userB, synergy);

                    List<CharBattle> finalParticipants = new List<CharBattle> { userA };
                    if (userB != null) finalParticipants.Add(userB);

                    foreach (var p in partnersToJoin)
                    {
                        finalParticipants.Add(p);
                    }

                    HideCommandMenu();
                    HideSubMenu();

                    TargetSelectionManager.instance.BeginTargetSelection(finalParticipants.ToArray(), synergy.Ability);
                }, SynergyButtonContainer); 
            }
        }
    }

    private void StoreTagForUser(PlayerCharBattle user, TriSynergyResult synergy)
    {
        user.GetStoredTags().Clear();
        if (user.charData.name == synergy.UserA.name)      user.GetStoredTags().Add(synergy.UserATag);
        else if (user.charData.name == synergy.UserB.name) user.GetStoredTags().Add(synergy.UserBTag);
        else                                               user.GetStoredTags().Add(synergy.UserCTag);
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
