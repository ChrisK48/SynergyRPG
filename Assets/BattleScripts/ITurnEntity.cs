using UnityEngine;

public interface ITurnEntity
{
    public string EntityName { get; }
    public int spd { get; }
    public bool entityIsPreppingSynergy { get; }
    public void TakeDamage(int amt, AtkType atkType, bool ignoreDef = false, System.Action<int> onDamageDealt = null);
    public void Heal(int amt);
    public void ReceiveBuff(Buff buff, int duration);
    public void ProcessTurnBuffs();
    public void StartPrep(Ability[] abilities);
    public void EndPrep();
}
