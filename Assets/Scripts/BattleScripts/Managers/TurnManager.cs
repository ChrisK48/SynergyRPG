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
    // Use the existing logic to find characters who !hasActed
    List<ITurnEntity> allChars = new List<ITurnEntity>();
    
    allChars.AddRange(BattleManager.instance.playerEntities.Where(pc => 
        pc is PlayerCharBattle player && player.GetIfAlive() && 
        !player.GetIfInSynergyStance() && !player.GetIfActed()));

    allChars.AddRange(BattleManager.instance.npcEntities.Where(npc => 
        npc is NpcBattle enemy && enemy.GetIfAlive() && 
        !enemy.GetIfInSynergyStance() && !enemy.GetIfActed()));

    var activeStances = BattleManager.instance.GetSynergyStances();
    if (activeStances != null)
        allChars.AddRange(activeStances);

    allChars.Sort((a, b) => b.spd.CompareTo(a.spd));

    turnOrder = allChars;

    // FIX: If we rebuild the list mid-round, reset the index to 0.
    // Since we filtered by '!GetIfActed()', index 0 will ALWAYS be 
    // the fastest person who still needs a turn.
    currentTurn = 0;

    Debug.Log("Turn order created: " + string.Join(", ", turnOrder.Select(e => e.EntityName)));
    return turnOrder;
}

public ITurnEntity getCurrentChar()
{
    // If the list is empty or we finished the round, reset everything
    if (turnOrder == null || turnOrder.Count == 0 || currentTurn >= turnOrder.Count)
    {
        // Full Round Reset
        currentTurn = 0;
        foreach (var p in BattleManager.instance.playerEntities) p.SetHasActed(false);
        foreach (var n in BattleManager.instance.npcEntities) n.SetHasActed(false);
        
        createTurnOrder(); 
    }

    // Double check to prevent crash if everyone is dead/in stances
    if (turnOrder.Count == 0) return null;

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

    public void HandleStanceCancel(SynergyStance stance, CharBattle faster, CharBattle slower)
    {
        turnOrder.Remove(stance);

        if (!BattleManager.instance.playerEntities.Contains(faster)) BattleManager.instance.playerEntities.Add(faster);
        if (!BattleManager.instance.playerEntities.Contains(slower)) BattleManager.instance.playerEntities.Add(slower);

        createTurnOrder();

        turnOrder.Remove(faster);
        turnOrder.Insert(0, faster);

        currentTurn = 0;
        
        Debug.Log($"Stance cancelled. {faster.CharName} fast-tracked to current turn.");
    }
}
