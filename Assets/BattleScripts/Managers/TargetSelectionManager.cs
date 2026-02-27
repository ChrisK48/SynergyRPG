using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.UI;
public class TargetSelectionManager : MonoBehaviour
{
    public static TargetSelectionManager instance;
    public Button TargetPopupPrefab;
    public Transform TargetPopupContainer;

    public void Awake()
    {
        instance = this;
    }

    public void BeginTargetSelection(CharBattle[] users, ITargetableAction action)
    {
        List<CharBattle> targets = new List<CharBattle>();
        TargetType targetType = action.Targets;

        switch (targetType)
        {
            case TargetType.SingleEnemy:
                targets.AddRange(BattleManager.instance.npcChars.Where(npc => npc.GetIfAlive()));
                break;
            case TargetType.AllEnemies:
                targets.AddRange(BattleManager.instance.npcChars.Where(npc => npc.GetIfAlive()));
                break;
            case TargetType.SingleAlly:
                targets.AddRange(BattleManager.instance.playerChars.Where(pc => pc.GetIfAlive()));
                break;
            case TargetType.AllAllies:
                targets.AddRange(BattleManager.instance.playerChars.Where(pc => pc.GetIfAlive()));
                break;
            case TargetType.Self:
                targets.AddRange(users);
                break;
            case TargetType.AnyChar:
                targets.AddRange(BattleManager.instance.playerChars.Where(pc => pc.GetIfAlive()));
                targets.AddRange(BattleManager.instance.npcChars.Where(npc => npc.GetIfAlive()));
                break;
            case TargetType.AllChars:
                targets.AddRange(BattleManager.instance.playerChars.Where(pc => pc.GetIfAlive()));
                targets.AddRange(BattleManager.instance.npcChars.Where(npc => npc.GetIfAlive()));
                break;
        }

        ShowPopups(users, action, targets);
    }

    void ShowPopups(CharBattle[] users, ITargetableAction action, List<CharBattle> targets)
    {
        foreach (CharBattle target in targets)
        {
            BattleUIManager.instance.commandMenuUIContainer.gameObject.SetActive(false);
            Button btn = Instantiate(TargetPopupPrefab, TargetPopupContainer);
            Vector3 screenPos = Camera.main.WorldToScreenPoint(target.transform.position + Vector3.up);
            btn.GetComponent<RectTransform>().position = screenPos;
            btn.onClick.AddListener(() => OnTargetSelected(users, action, target));
        }
    }

    void ClearPopups()
    {
        foreach (Transform child in TargetPopupContainer)
        {
            Destroy(child.gameObject);
        }
    }

    void OnTargetSelected(CharBattle[] users, ITargetableAction action, CharBattle target)
    {
        Debug.Log("Target selected: " + target.CharName);
        List<CharBattle> targets = new List<CharBattle>();

        switch (action.Targets)
        {
            case TargetType.SingleEnemy:
                targets.Add(target);
                break;
            case TargetType.SingleAlly:
                targets.Add(target);
                break;
            case TargetType.AllEnemies:
                targets.AddRange(BattleManager.instance.npcChars);
                break;
            case TargetType.AllAllies:
                targets.AddRange(BattleManager.instance.playerChars);
                break;
            case TargetType.Self:
                targets.AddRange(users);
                break;
            case TargetType.AnyChar:
                targets.Add(target);
                break;
            case TargetType.AllChars:
                targets.AddRange(BattleManager.instance.playerChars);
                targets.AddRange(BattleManager.instance.npcChars);
                break;
        }

        Debug.Log("Users: " + string.Join(", ", users.Select(u => u.CharName)));
        action.PerformAction(users, targets);
        ClearPopups();
        BattleUIManager.instance.commandMenuUIContainer.gameObject.SetActive(true);
        BattleManager.instance.NextTurn();
    }
}