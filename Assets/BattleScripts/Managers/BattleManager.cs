using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;
    public List<PlayerCharBattle> playerChars;
    public List<NpcBattle> npcChars;
    public List<ITurnEntity> playerEntities = new List<ITurnEntity>();
    public List<ITurnEntity> npcEntities = new List<ITurnEntity>();
    public List<Transform> playerSpawnPoints;

    public List<Transform> npcSpawnPoints;
    private TurnManager turnManager;
    private int earnedXp = 0;

     void Awake()
    {
        instance = this;
        turnManager = new TurnManager();
    }

    void Start()
    {

        List<PlayerCharBattle> spawnedPlayers = new List<PlayerCharBattle>();

        for (int i = 0; i < playerChars.Count; i++)
        {
            // Spawn the clone
            Transform spawnPoint = playerSpawnPoints[i];
            PlayerCharBattle clone = Instantiate(playerChars[i], spawnPoint.position, spawnPoint.rotation);
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

    public void NextTurn()
    {
        ITurnEntity currentEntity = turnManager.getCurrentChar();
        if (currentEntity is CharBattle currentChar)
        {
            if (!currentChar.GetIfAlive())
            {
                turnManager.AdvanceTurn(); 
                NextTurn();   
                return;
            }   
        }

        BattleUIManager.instance.UpdateTurnOrderUI(turnManager.GetTurnOrder(), currentEntity, turnManager.GetCurrentTurnIndex());

        currentEntity.ProcessTurnBuffs();
        CheckIfNotFollowedUp(currentEntity);

        turnManager.AdvanceTurn(); 

        if (currentEntity is PlayerCharBattle || currentEntity is SynergyStance)
        {
            BattleUIManager.instance.ShowCommandMenu(currentEntity);
        }
        else if (currentEntity is NpcBattle npc)
        {
            npc.PerformAITurn();
        }
        
        if (SynergyStanceManager.instance.GetIfStanceExists() && turnManager.getCurrentChar() is SynergyStance) FlowManager.instance.ConsumeFlow(10);
        CheckBattleEnd();
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

    private void CheckBattleEnd()
    {
        if (playerChars.All(pc => !pc.GetIfAlive()))
        {
            Debug.Log("All players defeated! Game Over.");
            Debug.Log("Total XP Earned: " + earnedXp);
        }
        else if (npcChars.All(npc => !npc.GetIfAlive()))
        {
            Debug.Log("All enemies defeated! Victory!");
            // Handle victory logic here
        }
    }

    public void AddEarnedXp(int xp)
    {
        earnedXp += xp;
    }

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

    public List<SynergyStance> GetSynergyStances()
    {
        return playerEntities.OfType<SynergyStance>().ToList();
    }
}
