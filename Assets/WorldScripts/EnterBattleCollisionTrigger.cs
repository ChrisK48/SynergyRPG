using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterBattleCollisionTrigger : MonoBehaviour, ICollisionTrigger
{
    public void OnPlayerCollision()
    {
        SceneManager.LoadScene("DefaultBattleScene");
    }
}
