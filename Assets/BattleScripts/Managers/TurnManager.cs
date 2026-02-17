using System.Collections.Generic;
using UnityEngine.Rendering;

public class TurnManager
{
    BattleManager battleManager;
    private List<CharBattle> turnOrder;
    private int currentTurn = 0;

    public TurnManager()
    {
        battleManager = BattleManager.instance;
    }

    public List<CharBattle> createTurnOrder()
    {
        turnOrder = new List<CharBattle>();
        List<CharBattle> allChars = new List<CharBattle>();
        allChars.AddRange(BattleManager.instance.playerChars);
        allChars.AddRange(BattleManager.instance.npcChars);

        allChars.Sort((a, b) => b.Spd.CompareTo(a.Spd));

        foreach (CharBattle character in allChars)
        {
            if (character.isAlive)
                turnOrder.Add(character);
        }

        BattleUIManager.instance.GenerateTurnOrderUI(turnOrder);
        return turnOrder;
    }

    public CharBattle getCurrentChar()
{
    // If we finished the list, reset and re-sort
    if (currentTurn >= turnOrder.Count)
    {
        currentTurn = 0;
        createTurnOrder(); 
    }
    return turnOrder[currentTurn];
}

    public void AdvanceTurn()
    {
        currentTurn++;
    }

    public List<CharBattle> GetTurnOrder()
    {
        return turnOrder;
    }

    public int GetCurrentTurnIndex()
    {
        return currentTurn;
    }
}
