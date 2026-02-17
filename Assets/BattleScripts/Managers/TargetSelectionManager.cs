using System.Collections.Generic;
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

    public void BeginTargetSelection(CharBattle user, Ability ability)
    {
        List<CharBattle> targets = new List<CharBattle>();
        TargetType targetType = ability.targetType;

        switch (targetType)
        {
            case TargetType.SingleEnemy:
                // Temp selection of first enemy
                targets.AddRange(BattleManager.instance.npcChars);
                break;
            case TargetType.AllEnemies:
                targets.AddRange(BattleManager.instance.npcChars);
                break;
            case TargetType.SingleAlly:
                // Temp selection of first ally
                targets.AddRange(BattleManager.instance.playerChars);
                break;
            case TargetType.AllAllies:
                targets.AddRange(BattleManager.instance.playerChars);
                break;
            case TargetType.Self:
                targets.Add(user);
                break;
            case TargetType.AnyChar:
                targets.AddRange(BattleManager.instance.playerChars);
                targets.AddRange(BattleManager.instance.npcChars);
                break;
            case TargetType.AllChars:
                targets.AddRange(BattleManager.instance.playerChars);
                targets.AddRange(BattleManager.instance.npcChars);
                break;
        }

        ShowPopups(user, ability, targets);
    }

    void ShowPopups(CharBattle user, Ability ability, List<CharBattle> targets)
    {
        foreach (CharBattle target in targets)
        {
            BattleUIManager.instance.commandMenuUIContainer.gameObject.SetActive(false);
            Button btn = Instantiate(TargetPopupPrefab, TargetPopupContainer);
            Vector3 screenPos = Camera.main.WorldToScreenPoint(target.transform.position + Vector3.up);
            btn.GetComponent<RectTransform>().position = screenPos;
            btn.onClick.AddListener(() => OnTargetSelected(user, ability, target));
        }
    }

    void ClearPopups()
    {
        foreach (Transform child in TargetPopupContainer)
        {
            Destroy(child.gameObject);
        }
    }

    void OnTargetSelected(CharBattle user, Ability ability, CharBattle target)
    {
        Debug.Log("Target selected: " + target.charName);
        List<CharBattle> targets = new List<CharBattle>();

        switch (ability.targetType)
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
                targets.Add(user);
                break;
            case TargetType.AnyChar:
                targets.Add(target);
                break;
            case TargetType.AllChars:
                targets.AddRange(BattleManager.instance.playerChars);
                targets.AddRange(BattleManager.instance.npcChars);
                break;
        }

        user.PerformAbility(ability, targets);
        ClearPopups();
        BattleUIManager.instance.commandMenuUIContainer.gameObject.SetActive(true);
        BattleManager.instance.NextTurn();
    }
}