using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;
    [HideInInspector]
    public List<PlayerCharBattle> playerChars;
    public List<NpcBattle> npcChars;
    public List<ITurnEntity> playerEntities = new List<ITurnEntity>();
    public List<ITurnEntity> npcEntities = new List<ITurnEntity>();
    public List<Transform> playerSpawnPoints;
    public List<Transform> npcSpawnPoints;
    public List<DualSynergyResult> preppedDualSynergies = new List<DualSynergyResult>();
    private TurnManager turnManager;
    private int earnedXp = 0;
    private int earnedMoney = 0;

     void Awake()
    {
        instance = this;
        turnManager = new TurnManager();
    }

    void Start()
    {
        Camera.main.transform.position = new Vector3(0, 0, -10);

        List<PlayerCharData> partyMembers = PartyManager.instance.activePartyMembers;
        playerChars = partyMembers.Select(data => data.charBattlePrefab).ToList();
        List<PlayerCharBattle> spawnedPlayers = new List<PlayerCharBattle>();
        npcChars = BattleTransitionManager.instance.getCurrentBattleEnemies();

        for (int i = 0; i < playerChars.Count; i++)
        {
            // Spawn the clone
            Transform spawnPoint = playerSpawnPoints[i];
            PlayerCharBattle clone = Instantiate(playerChars[i], spawnPoint.position, spawnPoint.rotation);
            clone.InitializeStatsFromData(partyMembers[i]);
            spawnedPlayers.Add(clone);
            playerEntities.Add(clone);
            Debug.Log("Spawned player character: " + clone.CharName + " with HP: " + clone.getHp() + " and MP: " + clone.getMp());
        }

        List<NpcBattle> spawnedNpcs = new List<NpcBattle>();

        for (int i = 0; i < npcChars.Count; i++)        
        {
            // Spawn the clone
            Transform spawnPoint = npcSpawnPoints[i];
            NpcBattle ncpClone = Instantiate(npcChars[i], spawnPoint.position, spawnPoint.rotation);
            spawnedNpcs.Add(ncpClone);
            npcEntities.Add(ncpClone);
        }

        playerChars = spawnedPlayers;
        npcChars = spawnedNpcs;
        BattleUIManager.instance.GeneratePartyUI(playerChars);
        turnManager.createTurnOrder();
        NextTurn();
    }

    public void SpawnNpc(NpcBattle npcPrefab)
    {
        Debug.Log("Spawning NPC: " + npcPrefab.CharName);
        int npcIndex = npcChars.Count;
        Transform spawnPoint = npcSpawnPoints[npcIndex]; 
        NpcBattle npcClone = Instantiate(npcPrefab, spawnPoint.position, spawnPoint.rotation);
        npcChars.Add(npcClone);
        npcEntities.Add(npcClone);
        BattleUIManager.instance.UpdateTurnOrderUI(turnManager.GetTurnOrder(), turnManager.getCurrentChar(), turnManager.GetCurrentTurnIndex());
    }

    public void NextTurn()
    {
        StartCoroutine(NextTurnRoutine());
    }

    private System.Collections.IEnumerator NextTurnRoutine()
    {
        yield return new WaitForEndOfFrame();
        if (CheckBattleEnd())
        {
            HandleBattleEnd();
            yield break;
        }
        ITurnEntity currentEntity = turnManager.getCurrentChar();
        if (currentEntity is CharBattle currentChar)
        {
            if (!currentChar.GetIfAlive())
            {
                turnManager.AdvanceTurn(); 
                NextTurn();   
                yield break;
            }   
        }

        if (currentEntity.GetIfHiding()) 
        {
            currentEntity.RevealChar();
            if (turnManager.GetTurnOrder().Contains(currentEntity))
                turnManager.AdvanceTurn();
            NextTurn();
            yield break;
        }

        BattleUIManager.instance.UpdateTurnOrderUI(turnManager.GetTurnOrder(), currentEntity, turnManager.GetCurrentTurnIndex());

        CheckIfNotFollowedUp(currentEntity);

        if (currentEntity.GetIfDefending()) currentEntity.Defend();
        
        if (currentEntity is PlayerCharBattle || currentEntity is SynergyStance)
        {
            BattleUIManager.instance.ShowCommandMenu(currentEntity);
        }
        else if (currentEntity is NpcBattle npc)
        {
            npc.PerformAITurn();
        }
    }

    private void CheckIfNotFollowedUp(ITurnEntity currentChar)
    {
        if (currentChar.entityIsPreppingSynergy && currentChar is PlayerCharBattle player || (currentChar is SynergyStance stance && stance.GetIfPreppingSynergy()))
        {
            Debug.Log(currentChar.EntityName + " did not follow up on their synergy prep! Synergy failed.");
            FlowManager.instance.ConsumeFlow(20); // Arbitrary flow penalty for not following up on synergy
            currentChar.EndPrep();
        }
    }

    private bool CheckBattleEnd()
    {
        if (playerChars.All(pc => !pc.GetIfAlive()) || npcChars.All(npc => !npc.GetIfAlive()))
        {
            return true;
        }
        return false;
    }
    private void HandleBattleEnd()
    {
        if (playerChars.All(pc => !pc.GetIfAlive()))
        {
            Debug.Log("All players defeated! Game Over.");
            Debug.Log("Total XP Earned: " + earnedXp);
        }
        else if (npcChars.All(npc => !npc.GetIfAlive()))
        {
            Debug.Log("All enemies defeated! Victory!");
            BattleUIManager.instance.ShowVictoryScreen(earnedXp, earnedMoney);
        }
    }

    public void AddEarnedXp(int xp)
    {
        Debug.Log("Adding earned XP: " + xp);
        earnedXp += xp;
    }

    public void AddEarnedMoney(int amount)
    {
        Debug.Log("Adding earned money: " + amount);
        earnedMoney += amount;
    }

    public int GetEarnedXp() => earnedXp;
    public int GetEarnedMoney() => earnedMoney;

    public TurnManager GetTurnManager()
    {
        return turnManager;
    }

    public void InsertSynergyStance(SynergyStance stance)
    {
        playerEntities.Add(stance);
        playerEntities.RemoveAll(entity => stance.users.Contains(entity));
        turnManager.InsertSynergy(stance);
    }

    public void RemoveSynergyStance(SynergyStance stance)
    {
        playerEntities.Remove(stance);
        playerEntities.AddRange(stance.users);
        turnManager.RemoveFromTurnOrder(stance);
    }

    public List<SynergyStance> GetSynergyStances()
    {
        return playerEntities.OfType<SynergyStance>().ToList();
    }
}
