using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleTransitionManager : MonoBehaviour
{
    private Vector3 playerWorldPosition;
    private string lastSceneName;
    public static BattleTransitionManager instance;
    private List<NpcBattle> currentBattleEnemies;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public Vector3 getPlayerWorldPosition() => playerWorldPosition;
    public void ClearPlayerWorldPosition() => playerWorldPosition = Vector3.zero;

    public List<NpcBattle> getCurrentBattleEnemies()
    {
        return currentBattleEnemies;
    }

    public void EnterBattle(List<NpcBattle> enemies)
    {
        playerWorldPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        lastSceneName = SceneManager.GetActiveScene().name;
        currentBattleEnemies = enemies;
        SceneManager.LoadScene("DefaultBattleScene");
    }

    public void ExitBattle()
    {
        SceneManager.LoadScene(lastSceneName);
    }
}
