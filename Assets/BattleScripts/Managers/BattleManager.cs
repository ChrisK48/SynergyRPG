using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;
    public List<PlayerCharBattle> playerChars;
    public List<NpcBattle> npcChars;
    private TurnManager turnManager;

     void Awake()
    {
        instance = this;
    }

    void Start()
    {
        turnManager = new TurnManager();

        List<PlayerCharBattle> spawnedPlayers = new List<PlayerCharBattle>();

        foreach (PlayerCharBattle playerChar in playerChars)
        {
            // Spawn the clone
            PlayerCharBattle clone = Instantiate(playerChar);
            spawnedPlayers.Add(clone);
        }

        List<NpcBattle> spawnedNpcs = new List<NpcBattle>();

        foreach (NpcBattle npcChar in npcChars)
        {
            // Spawn the clone
            NpcBattle ncpClone = Instantiate(npcChar);
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
