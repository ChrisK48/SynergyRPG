using UnityEngine;
using UnityEngine.SceneManagement;
public class WorldTransitionManager : MonoBehaviour
{
    public GameObject Player;
    private string DoorName;
    public static WorldTransitionManager instance;
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

    public void MoveToRoom(string RoomName, string DoorName)
    {
        this.DoorName = DoorName;
        SceneManager.LoadScene(RoomName);
    }

    public string GetDoorName() => DoorName;
}
