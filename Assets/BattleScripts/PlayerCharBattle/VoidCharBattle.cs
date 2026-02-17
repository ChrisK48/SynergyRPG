using UnityEngine;

public class VoidCharBattle : PlayerCharBattle
{
    public int voidPoints;
    public int maxVoidPoints;

    public void GainVoid(int amt)
    {
        voidPoints = Mathf.Clamp(voidPoints + amt, 0, maxVoidPoints);
    }
    public void LoseVoid(int amt)
    {
        voidPoints = Mathf.Clamp(voidPoints - amt, 0, maxVoidPoints);
    }
}
