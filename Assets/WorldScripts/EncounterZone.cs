using UnityEngine;
using System.Collections.Generic;

public class EncounterZone : MonoBehaviour
{
    public List<BattleGroup> zoneEncounters;
    public int encounterChance = 10;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStepCounter psc = other.GetComponent<PlayerStepCounter>();
            if (psc != null) psc.currentZone = this;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStepCounter psc = other.GetComponent<PlayerStepCounter>();
            if (psc != null && psc.currentZone == this) psc.currentZone = null;
        }
    }

    public BattleGroup GetBattleGroup()
    {
        if (zoneEncounters == null || zoneEncounters.Count == 0) return null;

        int totalWeight = 0;
        foreach (var bg in zoneEncounters)
        {
            totalWeight += bg.encounterChance;
        }

        int roll = Random.Range(0, totalWeight);
        int cursor = 0;

        for (int i = 0; i < zoneEncounters.Count; i++)
        {
            cursor += zoneEncounters[i].encounterChance;
            if (roll < cursor)
            {
                return zoneEncounters[i];
            }
        }

        return zoneEncounters[0];
    }
}