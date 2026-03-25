using UnityEngine;

// This is the script for the Steal effect. Currently it just rolls against the target's HP percentage. In future, I want to add item weights.
public class StealAbilityEffect : AbilityEffect
{
    public int baseStealChance;
    public override void ApplyEffect(CharBattle[] users, ITurnEntity target, int calculatedPower)
    {   
        if (target is NpcBattle npcTarget)
        {
            if (npcTarget.GetIfItemStolen())
            {
                Debug.Log($"(Already stolen from {npcTarget.CharName} in this battle)");
            }
            
            int roll = Random.Range(0, 100);
            int hpPercent = Mathf.RoundToInt((float)npcTarget.getHp() / npcTarget.MaxHp * 100f);
            float baseChance = 100 - hpPercent;   
            float hpTarget = baseChance - npcTarget.StealMultiplier;

            if (roll < hpTarget + baseStealChance && !npcTarget.GetIfItemStolen())
            {
                Debug.Log($"Stole {npcTarget.StealableItem.ItemName} from {npcTarget.CharName}");
                PartyManager.instance.GainItem(npcTarget.StealableItem);
                npcTarget.SetItemStolen();
            }
            else
            {
                Debug.Log($"Item couldn't be stolen from {npcTarget.CharName}");
            }
        }
    }
}
