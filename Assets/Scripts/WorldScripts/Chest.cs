using NUnit.Framework.Internal.Execution;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    public Item lootItem;
    private bool isOpened = false;
    public void OnPlayerInteraction(PlayerController player)
    {
        player.SetState(PlayerState.Dialogue);
        if (!isOpened)
        {
            isOpened = true;
            PartyManager.instance.GainItem(lootItem);
            TextManager.instance.ShowPopup(lootItem.ItemName);
        } else
        {
            TextManager.instance.ShowPopup("It's just an empty chest.");
        }
    }
}
