using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterBattleCollisionTrigger : MonoBehaviour, ICollisionTrigger
{
    public List<NpcBattle> enemies;
    public void OnPlayerCollision()
    {
        BattleTransitionManager.instance.EnterBattle(enemies);
        SceneManager.LoadScene("DefaultBattleScene");
    }
}
