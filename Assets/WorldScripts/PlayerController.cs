using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Transform InteractionPoint;
    private float MoveSpeed = 7f;
    private float SprintMultiplier = 1.8f;
    private Vector2 moveInput;
    void Update()
    {
        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            MenuManager.instance.ToggleMenu();
        }
        
        if (MenuManager.instance.GetIfMenuOpen()) 
        {
            moveInput = Vector2.zero;
            return;
        };

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
    }

    void FixedUpdate()
    {
        Vector2 inputDir = Vector2.ClampMagnitude(moveInput, 1f);

        float currentSpeed = MoveSpeed;
        if (Keyboard.current.leftShiftKey.isPressed) currentSpeed *= SprintMultiplier;
        GetComponent<Rigidbody2D>().linearVelocity = inputDir * currentSpeed;
    }
}
