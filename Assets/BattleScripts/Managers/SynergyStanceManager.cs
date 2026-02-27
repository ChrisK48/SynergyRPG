using UnityEngine;

public class SynergyStanceManager : MonoBehaviour
{
    public static SynergyStanceManager instance;

    public void Awake()
    {
        instance = this;
    }

    public void CreateSynergyStance(CharBattle[] users)
    {
        foreach (CharBattle user in users)
        {
            user.EnterSynergyStance();
        }
        BattleManager.instance.InsertSynergyStance(new SynergyStance(users));
        BattleManager.instance.NextTurn();
    }
}
