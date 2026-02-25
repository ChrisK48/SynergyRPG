using UnityEngine;

public class ShieldCharBattle : PlayerCharBattle
{
    public int maxShieldPoints = 10;
    public int shieldPoints = 0;

    public void GainShield(int amt)
    {
        shieldPoints = Mathf.Clamp(shieldPoints + amt, 0, maxShieldPoints);
        TriggerStatsUpdate();
    }

    public void LoseShield(int amt)
    {
        shieldPoints = Mathf.Clamp(shieldPoints - amt, 0, maxShieldPoints);
        TriggerStatsUpdate();
    }

    public bool CanPayShieldCost(int cost)
    {
        return shieldPoints >= cost;
    }
}
