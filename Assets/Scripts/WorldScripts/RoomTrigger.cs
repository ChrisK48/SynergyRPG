using UnityEngine;
public class RoomTrigger : MonoBehaviour, ICollisionTrigger
{
    public string RoomName;
    public string DoorName;
    public GameObject Spawn;
    public void OnPlayerCollision()
    {
        WorldTransitionManager.instance.MoveToRoom(RoomName, DoorName);
    }
}
