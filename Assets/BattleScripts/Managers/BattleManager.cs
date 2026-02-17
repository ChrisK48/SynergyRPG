using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;
    public List<PlayerCharBattle> playerChars;
    public List<CharBattle> enemyChars;
    private TurnManager turnManager;

     void Awake()
    {
        instance = this;
    }

    void Start()
    {
        turnManager = new TurnManager();

        List<PlayerCharBattle> spawnedPlayers = new List<PlayerCharBattle>();

        foreach (PlayerCharBattle pcPrefab in playerChars)
        {
            // Spawn the clone
            PlayerCharBattle clone = Instantiate(pcPrefab);
            spawnedPlayers.Add(clone);
        }

        playerChars = spawnedPlayers;
        BattleUIManager.instance.GeneratePartyUI(playerChars);
        turnManager.createTurnOrder();
        NextTurn();
    }

    public void NextTurn()
    {
        BattleUIManager.instance.ClearCommandMenu();
        CharBattle currentChar = turnManager.getNextTurn();
        Debug.Log("Current Turn: " + currentChar.charName);

        if (currentChar is PlayerCharBattle)
        {
            BattleUIManager.instance.ShowCommandMenu(currentChar);
        }
        else
        {
            // Enemy AI turn logic would go here
            Debug.Log("Enemy turn for: " + currentChar.charName);
        }

        for (int i = currentChar.activeBuffs.Count - 1; i >= 0; i--)
            {
                currentChar.activeBuffs[i].TickBuff(currentChar);
            }
    }
}
