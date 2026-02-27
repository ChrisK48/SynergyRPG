using UnityEngine;

public interface ITurnEntity
{
    public string entityName { get; }
    public int spd { get; }
    public bool entityIsPreppingSynergy { get; }
    public void ProcessTurnBuffs();
    public void StartPrep(Ability[] abilities);
    public void EndPrep();
}
