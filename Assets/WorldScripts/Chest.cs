using NUnit.Framework.Internal.Execution;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    public Item lootItem;
    private bool isOpened = false;
    public void OnPlayerInteraction()
    {
        if (!isOpened)
        {
            isOpened = true;
            PartyManager.instance.GainItem(lootItem);
            Debug.Log($"You opened the chest and found {lootItem.ItemName}!");
        } else
        {
            Debug.Log("It's just an empty chest.");
        }
    }
}
