using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{
    public static BattleUIManager instance;
    public Transform partyUIContainer;
    public Transform commandMenuUIContainer;
    public GameObject commandMenuPrefab;

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

    public void ShowCommandMenu(CharBattle pc)
    {
        GameObject cm = Instantiate(commandMenuPrefab, commandMenuUIContainer);

        // Position the command menu near the character (this needs to be changed but works for now)
        Vector3 screenPos = Camera.main.WorldToScreenPoint(pc.transform.position + Vector3.right);
        cm.GetComponent<RectTransform>().position = screenPos;

        Button attackBtn = cm.transform.Find("Attack").GetComponent<Button>();
        attackBtn.onClick.AddListener(() => 
            TargetSelectionManager.instance.BeginTargetSelection(pc, pc.abilities[0]));

        Button defendBtn = cm.transform.Find("Defend").GetComponent<Button>();
        defendBtn.onClick.AddListener(() => 
            TargetSelectionManager.instance.BeginTargetSelection(pc, pc.abilities[1]));
    }

    public void ClearCommandMenu()
    {
        foreach (Transform child in commandMenuUIContainer)
        {
            Destroy(child.gameObject);
        }
    }
}
