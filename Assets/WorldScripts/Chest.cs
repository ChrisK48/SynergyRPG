using NUnit.Framework.Internal.Execution;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    public ScriptableObject lootItem;
    private bool isOpened = false;
    public void OnPlayerInteraction()
    {
        if (!isOpened)
        {
            isOpened = true;
            if (lootItem is ConsumableItem consumable) PartyManager.instance.inventory.Add(new ItemStack { item = consumable, count = 1 });
            Debug.Log($"You opened the chest and found {lootItem.name}!");
        } else
        {
            Debug.Log("It's just an empty chest.");
        }
    }
}
