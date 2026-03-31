using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState { Roaming, Dialogue, Menu }

public class PlayerController : MonoBehaviour
{
    [Header("State")]
    public PlayerState currentState = PlayerState.Roaming;

    [Header("Movement")]
    public float MoveSpeed = 7f;
    public float SprintMultiplier = 1.8f;
    public Transform InteractionPoint;

    [Header("Interaction Settings")]
    public Vector2 boxSize = new Vector2(0.5f, 0.2f);
    public LayerMask InteractionLayer;

    private Vector2 moveInput;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        if (BattleTransitionManager.instance.getPlayerWorldPosition() != Vector3.zero)
        {
            transform.position = BattleTransitionManager.instance.getPlayerWorldPosition();
            BattleTransitionManager.instance.ClearPlayerWorldPosition();
        }
    }

    void Update()
    {
        switch (currentState)
        {
            case PlayerState.Roaming:
                HandleRoamingInput();
                break;
            case PlayerState.Dialogue:
                HandleDialogueInput();
                break;
            case PlayerState.Menu:
                HandleMenuInput();
                break;
        }
    }

    void HandleRoamingInput()
    {
        // 1. Menu Toggle
        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            SetState(PlayerState.Menu);
            MenuManager.instance.ToggleMenu();
            return;
        }

        // 2. Movement
        Vector2 tempInput = Vector2.zero;
        if (Keyboard.current.wKey.isPressed) tempInput.y += 1f;
        if (Keyboard.current.aKey.isPressed) tempInput.x -= 1f;
        if (Keyboard.current.sKey.isPressed) tempInput.y -= 1f;
        if (Keyboard.current.dKey.isPressed) tempInput.x += 1f;
        moveInput = tempInput;

        if (moveInput.sqrMagnitude > 0.01f) 
        {
            InteractionPoint.localPosition = moveInput.normalized * 0.5f;
        }

        // 3. Manual E Interaction (Signs/Chests)
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            CheckForInteraction();
        }
    }

    void HandleDialogueInput()
    {
        moveInput = Vector2.zero;
        rb.linearVelocity = Vector2.zero;

        // Proceed dialogue
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (TextManager.instance.IsDisplayingPopup())
            {
                TextManager.instance.HidePopup();
                SetState(PlayerState.Roaming);
            }

            TextManager.instance.DisplayNextSentence();
            
            // Return to roaming only when text is finished
            if (!TextManager.instance.IsDisplayingText())
            {
                SetState(PlayerState.Roaming);
            }
        }
    }

    void HandleMenuInput()
    {
        moveInput = Vector2.zero;
        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            MenuManager.instance.ToggleMenu();
            SetState(PlayerState.Roaming);
        }
    }

    void CheckForInteraction()
    {
        // Using your working Box Physics
        Collider2D hit = Physics2D.OverlapBox(InteractionPoint.position, boxSize, 0f, InteractionLayer);
        
        if (hit != null && hit.TryGetComponent(out IInteractable interactable))
        {
            // Passes 'this' to the sign to fix the NullReference error
            interactable.OnPlayerInteraction(this);
        }
    }

    // RESTORES YOUR OTHER INTERACTABLES (Portals, Auto-Triggers)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out ICollisionTrigger trigger)) 
        {
            trigger.OnPlayerCollision();
        }
    }

    public void SetState(PlayerState newState) => currentState = newState;

    void FixedUpdate()
    {
        if (currentState != PlayerState.Roaming) 
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 inputDir = Vector2.ClampMagnitude(moveInput, 1f);
        float currentSpeed = MoveSpeed * (Keyboard.current.leftShiftKey.isPressed ? SprintMultiplier : 1f);
        rb.linearVelocity = inputDir * currentSpeed;
    }

    // DEBUG: Shows the interaction box in Scene View
    private void OnDrawGizmosSelected()
    {
        if (InteractionPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(InteractionPoint.position, boxSize);
        }
    }
}