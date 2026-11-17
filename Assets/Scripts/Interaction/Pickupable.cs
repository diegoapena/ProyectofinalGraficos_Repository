using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Pickupable : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    [SerializeField] private string displayName = "Objeto";

    [Header("Hold")]
    [SerializeField] private Vector3 holdOffset = new Vector3(0f, -0.1f, 1.8f);
    [SerializeField] private Vector3 holdRotationEuler = Vector3.zero;
    [SerializeField] private float followSpeed = 14f;
    [SerializeField] private float followRotationSpeed = 10f;

    [Header("Events")]
    public UnityEvent OnPickedUpEvent = new UnityEvent();
    public UnityEvent OnReleasedEvent = new UnityEvent();
    public UnityEvent OnObjectDestroyedEvent = new UnityEvent();

    public event Action<Pickupable> PickedUp;
    public event Action<Pickupable> Released;
    public event Action<Pickupable, float> Thrown;
    public event Action<Pickupable> Destroyed;

    private Rigidbody body;
    private Collider[] itemColliders;
    private bool isHeld;

    public string InteractionPrompt => $"Presiona E para recoger {displayName}";
    public bool IsHeld => isHeld;
    public bool IsThrown { get; private set; }
    public float LastThrowPower { get; private set; }
    public Rigidbody Body => body;
    public Vector3 HoldOffset => holdOffset;
    public Quaternion HoldRotation => Quaternion.Euler(holdRotationEuler);
    public float FollowSpeed => followSpeed;
    public float FollowRotationSpeed => followRotationSpeed;

    protected virtual void Awake()
    {
        body = GetComponent<Rigidbody>();
        itemColliders = GetComponentsInChildren<Collider>();
    }

    public bool CanInteract(GameObject interactor)
    {
        return !isHeld;
    }

    public void Interact(GameObject interactor)
    {
        if (interactor.TryGetComponent(out PlayerCarry carry))
            carry.TryPickup(this);
    }

    public virtual void OnPickedUp()
    {
        isHeld = true;
        IsThrown = false;
        LastThrowPower = 0f;
        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        body.isKinematic = true;
        body.useGravity = false;
        SetCollidersEnabled(false);
        PickedUp?.Invoke(this);
        OnPickedUpEvent?.Invoke();
    }

    public virtual void OnReleased()
    {
        isHeld = false;
        body.isKinematic = false;
        body.useGravity = true;
        SetCollidersEnabled(true);
        Released?.Invoke(this);
        OnReleasedEvent?.Invoke();
    }

    public virtual void Throw(Vector3 direction, float impulse, float powerRating)
    {
        OnReleased();
        IsThrown = true;
        LastThrowPower = powerRating;
        body.AddForce(direction.normalized * impulse, ForceMode.Impulse);
        Thrown?.Invoke(this, powerRating);
    }

    protected void SetCollidersEnabled(bool enabled)
    {
        foreach (Collider itemCollider in itemColliders)
            itemCollider.enabled = enabled;
    }

    protected virtual void OnDestroy()
    {
        Destroyed?.Invoke(this);
        OnObjectDestroyedEvent?.Invoke();
    }
}
