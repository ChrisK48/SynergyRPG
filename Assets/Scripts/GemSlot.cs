using UnityEngine;

[System.Serializable]
public class GemSlot
{
    private Gem equippedGem;
    public GemType slotType;

    public void EquipGem(Gem newGem)
    {
        equippedGem = newGem;
    }

    public void UnequipGem()
    {
        equippedGem = null;
    }

    public Gem GetGem() => equippedGem;
}
