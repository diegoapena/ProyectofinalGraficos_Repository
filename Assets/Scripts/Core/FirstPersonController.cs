using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float jumpHeight = 1.2f;
    public float gravity = -19.62f;

    [Header("Look")]
    public Transform cameraTransform;
    public float mouseSensitivity = 0.12f;
    public float maxLookAngle = 85f;
    public bool handleEscape = true;

    private CharacterController controller;
    private float verticalVelocity;
    private float pitch;
    private bool cursorLocked;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        mouseSensitivity = Mathf.Clamp(PlayerPrefs.GetFloat("MouseSensitivity", mouseSensitivity), 0.02f, 0.4f);

        LockCursor(true);
    }

    private void Update()
    {
        Keyboard kb = Keyboard.current;
        Mouse mouse = Mouse.current;

        if (handleEscape && kb != null && kb.escapeKey.wasPressedThisFrame)
            LockCursor(!cursorLocked);

        if (mouse != null && mouse.leftButton.wasPressedThisFrame && !cursorLocked)
            LockCursor(true);

        if (!cursorLocked)
            return;

        if (mouse != null)
        {
            Vector2 look = mouse.delta.ReadValue() * mouseSensitivity;
            transform.Rotate(Vector3.up * look.x);
            pitch = Mathf.Clamp(pitch - look.y, -maxLookAngle, maxLookAngle);
            if (cameraTransform != null)
                cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        Vector2 input = Vector2.zero;
        if (kb != null)
        {
            if (kb.wKey.isPressed) input.y += 1f;
            if (kb.sKey.isPressed) input.y -= 1f;
            if (kb.dKey.isPressed) input.x += 1f;
            if (kb.aKey.isPressed) input.x -= 1f;
        }

        input = Vector2.ClampMagnitude(input, 1f);
        bool sprinting = kb != null && kb.leftShiftKey.isPressed;
        float speed = sprinting ? sprintSpeed : walkSpeed;
        Vector3 horizontalMove = (transform.right * input.x + transform.forward * input.y) * speed;

        if (controller.isGrounded)
        {
            if (verticalVelocity < 0f)
                verticalVelocity = -2f;

            if (kb != null && kb.spaceKey.wasPressedThisFrame)
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        verticalVelocity += gravity * Time.deltaTime;
        controller.Move(new Vector3(horizontalMove.x, verticalVelocity, horizontalMove.z) * Time.deltaTime);
    }

    private void LockCursor(bool locked)
    {
        cursorLocked = locked;
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}
