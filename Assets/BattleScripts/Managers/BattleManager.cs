using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;
    public List<PlayerCharBattle> playerChars;
    public List<NpcBattle> npcChars;
    public List<Transform> playerSpawnPoints;
    public List<Transform> npcSpawnPoints;
    private TurnManager turnManager;

     void Awake()
    {
        instance = this;
    }

    void Start()
    {
        turnManager = new TurnManager();

        List<PlayerCharBattle> spawnedPlayers = new List<PlayerCharBattle>();

        for (int i = 0; i < playerChars.Count; i++)
        {
            // Spawn the clone
            Transform spawnPoint = playerSpawnPoints[i];
            PlayerCharBattle clone = Instantiate(playerChars[i], spawnPoint.position, spawnPoint.rotation);
            spawnedPlayers.Add(clone);
        }

        List<NpcBattle> spawnedNpcs = new List<NpcBattle>();

        for (int i = 0; i < npcChars.Count; i++)        
        {
            // Spawn the clone
            Transform spawnPoint = npcSpawnPoints[i];
            NpcBattle ncpClone = Instantiate(npcChars[i], spawnPoint.position, spawnPoint.rotation);
            spawnedNpcs.Add(ncpClone);
        }

        playerChars = spawnedPlayers;
        npcChars = spawnedNpcs;
        BattleUIManager.instance.GeneratePartyUI(playerChars);
        turnManager.createTurnOrder();
        NextTurn();
    }

    public void NextTurn()
    {
        BattleUIManager.instance.ClearCommandMenu();

        CharBattle currentChar = turnManager.getCurrentChar();
        
        BattleUIManager.instance.UpdateTurnOrderUI(turnManager.GetTurnOrder(), currentChar, turnManager.GetCurrentTurnIndex());

        turnManager.AdvanceTurn();

        currentChar.ProcessTurnBuffs();

        if (!currentChar.isAlive)
        {
            NextTurn();
            return;
        }

        if (currentChar is PlayerCharBattle)
        {
            BattleUIManager.instance.ShowCommandMenu(currentChar);
        }
        else if (currentChar is NpcBattle npc)
        {
            npc.PerformAITurn();
        }
        CheckBattleEnd();
    }

    private void CheckBattleEnd()
    {
        if (playerChars.All(pc => !pc.isAlive))
        {
            Debug.Log("All players defeated! Game Over.");
            // Handle game over logic here
        }
        else if (npcChars.All(npc => !npc.isAlive))
        {
            Debug.Log("All enemies defeated! Victory!");
            // Handle victory logic here
        }
    }
}
