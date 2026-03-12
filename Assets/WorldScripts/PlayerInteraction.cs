using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerInteraction : MonoBehaviour
{
    public Transform InteractionPoint;
    private Vector2 boxSize = new Vector2(0.5f, 0.2f);
    [SerializeField] private LayerMask interactableLayer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out ICollisionTrigger trigger)) trigger.OnPlayerCollision();
    }

    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Collider2D collision = Physics2D.OverlapBox(InteractionPoint.position, boxSize, 0f, interactableLayer);
            if (collision != null && collision.TryGetComponent(out IInteractable interactable))
            {
                interactable.OnPlayerInteraction();
            }
        }

        // This will tell us EXACTLY which object is broken
    if (InteractionPoint == null) 
    {
        Debug.LogError("Found the ghost! The broken script is on: " + gameObject.name, gameObject);
        return; 
    }
    }
}