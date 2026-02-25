using UnityEngine;

[System.Serializable]
public abstract class BuffEffect
{
    public virtual void StartBuff(CharBattle target, ActiveBuff buffWrapper)
    {
        
    }
    public virtual void TickBuff(CharBattle target, ActiveBuff buffWrapper)
    {

    }
    public virtual void EndBuff(CharBattle target, ActiveBuff buffWrapper)
    {
        
    }
}
