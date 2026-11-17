using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask interactionMask = ~0;

    public event Action<IInteractable> OnHoverStart;
    public event Action<IInteractable> OnHoverEnd;

    private PlayerCarry carry;
    private IInteractable focused;

    public IInteractable Focused => focused;

    private void Awake()
    {
        carry = GetComponent<PlayerCarry>();
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        UpdateFocus();

        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && keyboard.eKey.wasPressedThisFrame && focused != null && !carry.IsCarrying)
            focused.Interact(gameObject);
    }

    private void UpdateFocus()
    {
        IInteractable newFocus = null;

        if (!carry.IsCarrying && cameraTransform != null)
        {
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, interactionRange, interactionMask, QueryTriggerInteraction.Collide))
            {
                IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
                if (interactable != null && interactable.CanInteract(gameObject))
                    newFocus = interactable;
            }
        }

        if (!ReferenceEquals(newFocus, focused))
        {
            if (focused != null)
                OnHoverEnd?.Invoke(focused);

            focused = newFocus;

            if (focused != null)
                OnHoverStart?.Invoke(focused);
        }
    }
}
