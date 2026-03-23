using UnityEngine;

public class PlayerStepCounter : MonoBehaviour
{
    public EncounterZone currentZone;
    public float distancePerStep = 2.0f; // Distance to walk before rolling
    private float distanceWalked = 0f;
    private Vector3 lastPos;

    void Start() => lastPos = transform.position;

    void Update()
    {
        float moveDist = Vector3.Distance(transform.position, lastPos);
        
        if (moveDist > 0 && currentZone != null)
        {
            distanceWalked += moveDist;

            if (distanceWalked >= distancePerStep)
            {
                distanceWalked = 0;
                TryRollEncounter();
            }
        }
        lastPos = transform.position;
    }

    void TryRollEncounter()
    {
        if (Random.Range(1, 101) <= currentZone.encounterChance)
        {
            Debug.Log("Encounter Triggered!");
            BattleGroup group = currentZone.GetBattleGroup();
            BattleTransitionManager.instance.EnterBattle(group.enemies);
        }
    }
}