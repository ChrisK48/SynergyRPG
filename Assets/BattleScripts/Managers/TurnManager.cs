using System.Collections.Generic;

public class TurnManager
{
    BattleManager battleManager;
    private Queue<CharBattle> turnOrder;

    public TurnManager()
    {
        battleManager = BattleManager.instance;
    }

    public Queue<CharBattle> createTurnOrder()
    {
        turnOrder = new Queue<CharBattle>();
        List<CharBattle> allChars = new List<CharBattle>();
        allChars.AddRange(BattleManager.instance.playerChars);
        allChars.AddRange(BattleManager.instance.npcChars);

        allChars.Sort((a, b) => b.Spd.CompareTo(a.Spd));

        foreach (CharBattle character in allChars)
        {
            turnOrder.Enqueue(character);
        }

        return turnOrder;
    }

    public CharBattle getNextTurn()
    {
        CharBattle nextChar = turnOrder.Dequeue();
        turnOrder.Enqueue(nextChar);
        return nextChar;
    }
}
