using UnityEngine;

public class InteractableSign : MonoBehaviour, IInteractable
{
    public string[] SignText;
    public void OnPlayerInteraction(PlayerController player)
    {
        player.SetState(PlayerState.Dialogue);     
        TextManager.instance.ShowMessage(SignText);
    }
}
