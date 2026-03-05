using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
        allChars.AddRange(BattleManager.instance.playerChars.Where(pc => pc.GetIfAlive()));
        allChars.AddRange(BattleManager.instance.npcChars.Where(npc => npc.GetIfAlive()));
        if (BattleManager.instance.GetSynergyStances() != null)
            allChars.AddRange(BattleManager.instance.GetSynergyStances());

        allChars.Sort((a, b) => b.spd.CompareTo(a.spd));

        foreach (ITurnEntity entity in allChars)
        {
            if (entity is NpcBattle npc)
            {
                if (npc.IsShieldBroken())
                {
                    npc.ResetShields();
                }
            }
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

    public void RemoveFromTurnOrder(ITurnEntity entity)
    {
        turnOrder.Remove(entity);
        if (currentTurn >= turnOrder.Count)
        {
            currentTurn = 0;
        }
        BattleUIManager.instance.UpdateTurnOrderUI(turnOrder, getCurrentChar(), currentTurn);
    }

    public void InsertSynergy(ITurnEntity synergyEntity)
    {
        if (synergyEntity is SynergyStance stance)
        {
            foreach (var user in stance.users)
            {
                int index = turnOrder.FindIndex(entity =>
                    entity is CharBattle character && (object)character == (object)user);

                if (index >= 0)
                {
                    if (index < currentTurn)
                    {
                        currentTurn--; // Adjust index shift
                    }

                    turnOrder.RemoveAt(index);
                }
            }
        }

        if (currentTurn > turnOrder.Count)
        {
            currentTurn = turnOrder.Count;
        }

        turnOrder.Insert(currentTurn, synergyEntity);

        BattleUIManager.instance.UpdateTurnOrderUI(turnOrder, getCurrentChar(), currentTurn);
    }
}
