using UnityEngine;
[System.Serializable]
public class ShieldTag
{
    public DamageType element;
    public int MaxShieldAmount;
    [HideInInspector]
    public int shieldAmount;

    public void ResetShieldAmount()
    {
        shieldAmount = MaxShieldAmount;
    }
}
