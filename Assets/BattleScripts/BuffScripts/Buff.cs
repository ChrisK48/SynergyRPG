using System.Collections.Generic;
using UnityEngine;

public abstract class Buff : ScriptableObject
{
    public string buffName;
    public string description;

    public virtual void StartBuff(CharBattle target, ActiveBuff buffWrapper)
    {
    }
    public virtual void TickBuff(CharBattle target, ActiveBuff buffWrapper)
    {

    }
    public virtual void EndBuff(CharBattle target, ActiveBuff buffWrapper)
    {
        Debug.Log(target.charName + "'s " + buffName + " buff has ended.");
    }
}

[System.Serializable]
public class ActiveBuff
{
    public Buff buff;
    public int remainingDuration;
    public Dictionary<string, int> statChanges = new Dictionary<string, int>();

    public ActiveBuff(Buff template, int duration)
    {
        buff = template;
        remainingDuration = duration;
    }

    public void Tick(CharBattle target)
    {
        remainingDuration--;
        buff.TickBuff(target, this); 
        Debug.Log(target.charName + "'s " + buff.buffName + " buff has " + remainingDuration + " turns remaining.");
        
        if (remainingDuration <= 0)
        {
            buff.EndBuff(target, this);
        }
    }
}