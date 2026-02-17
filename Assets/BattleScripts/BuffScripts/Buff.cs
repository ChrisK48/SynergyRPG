using UnityEngine;

public abstract class Buff : ScriptableObject
{
    public string buffName;
    public string description;
    public int duration;

    public abstract void StartBuff(CharBattle target);
    public virtual void TickBuff(CharBattle target)
    {
        duration--;
        if (duration <= 0)
        {
            EndBuff(target);
        }
        Debug.Log(target.charName + "'s " + buffName + " buff ticks. " + duration + " turns remaining.");
    }
    public virtual void EndBuff(CharBattle target)
    {
        Debug.Log(target.charName + "'s " + buffName + " buff has ended.");
    }
}
