using UnityEngine;

[System.Serializable]
public abstract class UniqueResourceCost
{
    public int amount;
    public abstract void PayCost(CharBattle user);
    public virtual bool CanPayCost(CharBattle user){ return true; }
}

[System.Serializable]
public class voidCost : UniqueResourceCost
{
    public override void PayCost(CharBattle user)
    {
        if (user is VoidCharBattle voidUser) 
            if (amount > 0) voidUser.GainVoid(amount);
            else voidUser.LoseVoid(-amount);
    }
}

[System.Serializable]
public class shieldCost : UniqueResourceCost
{
    public override void PayCost(CharBattle user)
    {
        if (user is ShieldCharBattle shieldUser) 
            if (amount > 0) shieldUser.GainShield(amount);
            else shieldUser.LoseShield(-amount);
    }

    public override bool CanPayCost(CharBattle user)
    {
        if (user is ShieldCharBattle shieldUser) 
            return shieldUser.CanPayShieldCost(amount);
        return false;
    }
}
