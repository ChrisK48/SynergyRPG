using UnityEngine;

public class VoidCharBattle : PlayerCharBattle
{
    public int maxVoidPoints = 100;
    public int voidPoints = 0;

    public void GainVoid(int amt)
    {
        voidPoints = Mathf.Clamp(voidPoints + amt, 0, maxVoidPoints);
        TriggerStatsUpdate();
    }
    public void LoseVoid(int amt)
    {
        voidPoints = Mathf.Clamp(voidPoints - amt, 0, maxVoidPoints);
        TriggerStatsUpdate();
    }
}
