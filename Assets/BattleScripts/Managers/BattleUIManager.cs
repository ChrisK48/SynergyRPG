using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    void Awake()
    {
        instance = this;
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

    public void UpdateTurnOrderUI(List<CharBattle> turnOrder, CharBattle currentChar, int currentTurnIndex)
    {
        foreach (Transform child in TurnOrderUIContainer)
        {
            Destroy(child.gameObject);
        }

        for(int i = currentTurnIndex; i < turnOrder.Count; i++)
        {
            GameObject turnOrderElemOther = Instantiate(turnOrderIconPrefab, TurnOrderUIContainer);
            turnOrderElemOther.GetComponentInChildren<TextMeshProUGUI>().text = turnOrder[i].charName;
        }
    }

    public void ShowCommandMenu(CharBattle pc)
    {
        GameObject cm = Instantiate(commandMenuPrefab, commandMenuUIContainer);

        // Position the command menu near the character (this needs to be changed but works for now)
        Vector3 screenPos = Camera.main.WorldToScreenPoint(pc.transform.position);
        screenPos.x += 125;
        screenPos.y -= 50;
        cm.GetComponent<RectTransform>().position = screenPos;

        Button attackBtn = cm.transform.Find("Attack").GetComponent<Button>();
        attackBtn.onClick.AddListener(() => 
            TargetSelectionManager.instance.BeginTargetSelection(pc, pc.abilities[0]));

        Button defendBtn = cm.transform.Find("Defend").GetComponent<Button>();
        defendBtn.onClick.AddListener(() => 
            TargetSelectionManager.instance.BeginTargetSelection(pc, pc.abilities[1]));

        Button abilityBtn = cm.transform.Find("Ability").GetComponent<Button>();
        abilityBtn.onClick.AddListener(() =>
        {
            GameObject abilityMenu = Instantiate(commandSubMenuPrefab, commandMenuUIContainer);
            for (int i = 2; i < pc.abilities.Count; i++)
            {
                GameObject abilityBtnSub = Instantiate(commandMenuButtonPrefab, abilityMenu.transform);
                Ability ability = pc.abilities[i];
                abilityBtnSub.GetComponentInChildren<TextMeshProUGUI>().text = ability.Name;
                abilityBtnSub.GetComponent<Button>().onClick.AddListener(() =>
                {
                    TargetSelectionManager.instance.BeginTargetSelection(pc, ability);
                    Destroy(abilityMenu);
                });
            }
        });

        Button itemBtn = cm.transform.Find("Item").GetComponent<Button>();
        itemBtn.onClick.AddListener(() =>
        {
            GameObject itemMenu = Instantiate(commandSubMenuPrefab, commandMenuUIContainer);
            foreach (var pair in InventoryManager.instance.items)
            {
                Item item = pair.Key;
                int count = pair.Value;

                GameObject itemBtnObj = Instantiate(commandMenuButtonPrefab, itemMenu.transform);
                // Show name and quantity: "Potion (x5)"
                itemBtnObj.GetComponentInChildren<TextMeshProUGUI>().text = $"{item.Name} (x{count})";

                itemBtnObj.GetComponent<Button>().onClick.AddListener(() =>
                {
                    // The Target Manager handles Items and Abilities exactly the same!
                    TargetSelectionManager.instance.BeginTargetSelection(pc, item);
                });
            }
        });
    }

    public void ClearCommandMenu()
    {
        foreach (Transform child in commandMenuUIContainer)
        {
            Destroy(child.gameObject);
        }
    }
}
