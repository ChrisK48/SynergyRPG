using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class TargetSelectionManager : MonoBehaviour
{
    public static TargetSelectionManager instance;
    public Button TargetPopupPrefab;
    public Transform TargetPopupContainer;
    public GameObject PlayerTargetIndicatorPrefab;
    public GameObject EnemyTargetIndicatorPrefab;

    public void Awake()
    {
        instance = this;
    }

    public void BeginTargetSelection(CharBattle[] users, ITargetableAction action)
    {
        List<ITurnEntity> targets = new List<ITurnEntity>();
        TargetType targetType = action.Targets;
        var activeStances = BattleManager.instance.GetSynergyStances();
        switch (targetType)
        {
            case TargetType.SingleEnemy:
                targets.AddRange(BattleManager.instance.npcChars.Where(npc => npc.GetIfAlive() && !activeStances.Any(stance => stance.users.Contains(npc))));
                break;
            case TargetType.AllEnemies:
                targets.AddRange(BattleManager.instance.npcChars.Where(npc => npc.GetIfAlive() && !activeStances.Any(stance => stance.users.Contains(npc))));
                break;
            case TargetType.SingleAlly:
                targets.AddRange(BattleManager.instance.playerChars.Where(pc => pc.GetIfAlive() && !activeStances.Any(stance => stance.users.Contains(pc))));
                targets.AddRange(BattleManager.instance.GetSynergyStances());
                break;
            case TargetType.AllAllies:
                targets.AddRange(BattleManager.instance.playerChars.Where(pc => pc.GetIfAlive() && !activeStances.Any(stance => stance.users.Contains(pc))));
                targets.AddRange(BattleManager.instance.GetSynergyStances());
                break;
            case TargetType.Self:
                if (users.Length > 1)
                {
                    targets.AddRange(BattleManager.instance.GetSynergyStances().Where(stance => stance.users.SequenceEqual(users)));
                    break;
                }
                targets.AddRange(users);
                break;
            case TargetType.AnyChar:
                targets.AddRange(BattleManager.instance.playerChars.Where(pc => pc.GetIfAlive() && !activeStances.Any(stance => stance.users.Contains(pc))));
                targets.AddRange(BattleManager.instance.npcChars.Where(npc => npc.GetIfAlive() && !activeStances.Any(stance => stance.users.Contains(npc))));
                targets.AddRange(BattleManager.instance.GetSynergyStances());
                break;
            case TargetType.AllChars:
                targets.AddRange(BattleManager.instance.playerChars.Where(pc => pc.GetIfAlive() && !activeStances.Any(stance => stance.users.Contains(pc))));
                targets.AddRange(BattleManager.instance.npcChars.Where(npc => npc.GetIfAlive() && !activeStances.Any(stance => stance.users.Contains(npc))));
                targets.AddRange(BattleManager.instance.GetSynergyStances());
                break;
            case TargetType.DeadAlly:
                targets.AddRange(BattleManager.instance.playerChars.Where(pc => !pc.GetIfAlive()));
                break;
            case TargetType.DeadAllies:
                targets.AddRange(BattleManager.instance.playerChars.Where(pc => !pc.GetIfAlive()));
                break;
        }

        ShowPopups(users, action, targets);
    }

    void ShowPopups(CharBattle[] users, ITargetableAction action, List<ITurnEntity> targets)
    {
        foreach (ITurnEntity target in targets)
        {
            Vector3 worldPos = GetTargetPosition(target);
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos + Vector3.up);
            if (target is PlayerCharBattle || target is SynergyStance stance && stance.users[0] is PlayerCharBattle)
            {
                GameObject PlayerTargetIndicatorPrefabInstance = Instantiate(PlayerTargetIndicatorPrefab, TargetPopupContainer);
                PlayerTargetIndicatorPrefabInstance.GetComponent<Button>().onClick.AddListener(() => OnTargetSelected(users, action, target));
                PlayerTargetIndicatorPrefabInstance.transform.position = screenPos;
               
            } else if (target is NpcBattle)
            {
                GameObject EnemyTargetIndicatorPrefabInstance = Instantiate(EnemyTargetIndicatorPrefab, TargetPopupContainer);
                EnemyTargetIndicatorPrefabInstance.GetComponent<Button>().onClick.AddListener(() => OnTargetSelected(users, action, target));
                EnemyTargetIndicatorPrefabInstance.transform.position = screenPos;
            }
            
        }
    }

    Vector3 GetTargetPosition(ITurnEntity entity)
    {
        if (entity is MonoBehaviour mb) return mb.transform.position;
        if (entity is SynergyStance stance) return stance.users[0].transform.position; // Or midpoint
        return Vector3.zero;
    }

    void ClearPopups()
    {
        foreach (Transform child in TargetPopupContainer)
        {
            Destroy(child.gameObject);
        }
    }

    public void OnTargetSelected(CharBattle[] users, ITargetableAction action, ITurnEntity target)
    {
        Debug.Log("Target selected: " + target.EntityName);
        List<ITurnEntity> targets = new List<ITurnEntity>();

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
            case TargetType.DeadAlly:
                targets.Add(target);
                break;
            case TargetType.DeadAllies:
                targets.AddRange(BattleManager.instance.playerChars.Where(pc => !pc.GetIfAlive()));
                break;
        }

        Debug.Log("Users: " + string.Join(", ", users.Select(u => u.EntityName)));
        action.PerformAction(users, targets);
        ClearPopups();
        if (users[0].GetIfInSynergyStance()) FlowManager.instance.ConsumeFlow(10);
        BattleManager.instance.NextTurn();
    }
}