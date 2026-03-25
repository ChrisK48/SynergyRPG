using UnityEngine;

public class RoomInit : MonoBehaviour
{
    void Start()
    {
        Vector3 playerPos = Vector3.zero;
        if (WorldTransitionManager.instance.GetDoorName() != null) 
        {
            string doorName = WorldTransitionManager.instance.GetDoorName();
            RoomTrigger[] doors = FindObjectsByType<RoomTrigger>(FindObjectsSortMode.None);
            Debug.Log($"Total doors found in scene: {doors.Length}");
            foreach (RoomTrigger door in doors)
            {
                if (door.DoorName == doorName)
                {
                    playerPos = door.Spawn.transform.position;
                    break;
                }
            }
        }
        Debug.Log($"Final Spawn Decision: {playerPos}");
        Instantiate(WorldTransitionManager.instance.Player, playerPos, Quaternion.identity);
    }
}
