using UnityEngine;

public class ShieldCharBattle : PlayerCharBattle
{
    public int maxShieldPoints;
    public int ShieldPoints;

    public void GainShield(int amt)
    {
        ShieldPoints = Mathf.Clamp(ShieldPoints + amt, 0, maxShieldPoints);
    }

    public void LoseShield(int amt)
    {
        ShieldPoints = Mathf.Clamp(ShieldPoints - amt, 0, maxShieldPoints);
    }
}
