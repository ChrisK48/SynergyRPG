using UnityEngine;
using Unity.Cinemachine;

public class RoomInit : MonoBehaviour
{
    void Start()
    {
        Vector3 playerPos = Vector3.zero;
        if (WorldTransitionManager.instance.GetDoorName() != null) 
        {
            string doorName = WorldTransitionManager.instance.GetDoorName();
            RoomTrigger[] doors = FindObjectsByType<RoomTrigger>(FindObjectsSortMode.None);
            foreach (RoomTrigger door in doors)
            {
                if (door.DoorName == doorName)
                {
                    playerPos = door.Spawn.transform.position;
                    break;
                }
            }
        }
        GameObject spawnedPlayer = Instantiate(WorldTransitionManager.instance.Player, playerPos, Quaternion.identity);
        CinemachineCamera vcam = FindFirstObjectByType<CinemachineCamera>();
        vcam.Follow = spawnedPlayer.transform;
    }
}
