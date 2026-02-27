using System.Collections.Generic;
using UnityEngine.Rendering;

public class TurnManager
{
    BattleManager battleManager;
    private List<ITurnEntity> turnOrder;
    private int currentTurn = 0;

    public TurnManager()
    {
        battleManager = BattleManager.instance;
    }

    public List<ITurnEntity> createTurnOrder()
    {
        turnOrder = new List<ITurnEntity>();
        List<ITurnEntity> allChars = new List<ITurnEntity>();
        allChars.AddRange(BattleManager.instance.playerChars);
        allChars.AddRange(BattleManager.instance.npcChars);
        if (BattleManager.instance.GetSynergyStances() != null)
            allChars.AddRange(BattleManager.instance.GetSynergyStances());

        allChars.Sort((a, b) => b.spd.CompareTo(a.spd));

        foreach (ITurnEntity entity in allChars)
        {
            if (entity is CharBattle character && character.GetIfAlive() && !character.GetIfInSynergyStance())
                turnOrder.Add(entity);
            else if (entity is SynergyStance synergy)
                turnOrder.Add(entity);
        }
        return turnOrder;
    }

    public ITurnEntity getCurrentChar()
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

    public List<ITurnEntity> GetTurnOrder()
    {
        return turnOrder;
    }

    public int GetCurrentTurnIndex()
    {
        return currentTurn;
    }

    public void InsertSynergy(ITurnEntity synergyEntity)
    {
        if (synergyEntity is SynergyStance stance)
        {
            // 1. Remove the characters FIRST
            // They are no longer individual actors, so they leave the list entirely.
            foreach (var user in stance.users)
            {
                turnOrder.RemoveAll(entity => 
                    entity is CharBattle character && (object)character == (object)user);
            }
        }

        // 2. Insert the synergy stance at the current turn index NOW.
        // Since the users are gone, this stance is guaranteed to be the next active turn.
        turnOrder.Insert(currentTurn, synergyEntity);
        BattleUIManager.instance.UpdateTurnOrderUI(turnOrder, getCurrentChar(), currentTurn);
    }
}
