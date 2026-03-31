using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Buff", menuName = "Buff")]
public class Buff : ScriptableObject
{
    public string buffName;
    public string description;
    public Sprite Icon;

    [SerializeReference]
    public BuffEffect buffEffect;

    public virtual void StartBuff(CharBattle target, ActiveBuff buffWrapper)
    {
        buffEffect.StartBuff(target, buffWrapper);
        Debug.Log(target.CharName + " has started " + buffName + " buff.");
    }
    public virtual void TickBuff(CharBattle target, ActiveBuff buffWrapper)
    {
        buffEffect.TickBuff(target, buffWrapper);
    }
    public virtual void EndBuff(CharBattle target, ActiveBuff buffWrapper)
    {
        buffEffect.EndBuff(target, buffWrapper);
        Debug.Log(target.CharName + "'s " + buffName + " buff has ended.");
    }
}

[System.Serializable]
public class ActiveBuff
{
    public Buff buff;
    public int remainingDuration;
    public bool justApplied = false;
    public Dictionary<string, int> statChanges = new Dictionary<string, int>();

    public ActiveBuff(Buff template, int duration)
    {
        buff = template;
        remainingDuration = duration;
    }

    public void Tick(CharBattle target)
    {
        if (justApplied)
        {
            justApplied = false;
            return;
        }

        remainingDuration--;
        buff.TickBuff(target, this); 
        Debug.Log(target.CharName + "'s " + buff.buffName + " buff has " + remainingDuration + " turns remaining.");
        
        if (remainingDuration <= 0)
        {
            buff.EndBuff(target, this);
        }
    }
}