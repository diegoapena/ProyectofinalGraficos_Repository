using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerCarry : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    [Header("Throw")]
    [SerializeField] private float minThrowPower = 1f;
    [SerializeField] private float maxThrowPower = 10f;
    [SerializeField] private float powerToImpulse = 1.6f;
    [SerializeField] private float maxChargeTime = 1.25f;
    [SerializeField] private float tapThreshold = 0.15f;

    [Header("Events")]
    public UnityEvent<float> OnChargeChangedEvent = new UnityEvent<float>();

    public event Action<float> OnChargeChanged;
    public event Action<Pickupable> OnObjectPickedUp;
    public event Action<Pickupable> OnObjectReleased;

    private Pickupable held;
    private bool charging;
    private float chargeStartTime;

    public bool IsCarrying => held != null;
    public Pickupable Held => held;
    public bool IsCharging => charging;
    public float ChargeNormalized => charging ? Mathf.Clamp01((Time.time - chargeStartTime) / maxChargeTime) : 0f;
    public float CurrentPower => Mathf.Lerp(minThrowPower, maxThrowPower, ChargeNormalized);

    private void Awake()
    {
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        if (held == null)
            return;

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
            return;

        if (keyboard.fKey.wasPressedThisFrame)
        {
            charging = true;
            chargeStartTime = Time.time;
        }

        if (charging)
        {
            OnChargeChanged?.Invoke(ChargeNormalized);
            OnChargeChangedEvent?.Invoke(ChargeNormalized);
        }

        if (keyboard.fKey.wasReleasedThisFrame && charging)
        {
            charging = false;
            float heldTime = Time.time - chargeStartTime;

            if (heldTime <= tapThreshold)
                Drop();
            else
                Throw(Mathf.Clamp01(heldTime / maxChargeTime));

            NotifyChargeChanged(0f);
        }
    }

    private void LateUpdate()
    {
        if (held == null)
            return;

        Transform reference = cameraTransform != null ? cameraTransform : transform;
        Vector3 targetPosition = reference.TransformPoint(held.HoldOffset);
        Quaternion targetRotation = reference.rotation * held.HoldRotation;

        float positionLerp = 1f - Mathf.Exp(-held.FollowSpeed * Time.deltaTime);
        float rotationLerp = 1f - Mathf.Exp(-held.FollowRotationSpeed * Time.deltaTime);

        held.transform.position = Vector3.Lerp(held.transform.position, targetPosition, positionLerp);
        held.transform.rotation = Quaternion.Slerp(held.transform.rotation, targetRotation, rotationLerp);
    }

    public bool TryPickup(Pickupable item)
    {
        if (item == null || held != null || item.IsHeld)
            return false;

        held = item;
        charging = false;
        held.OnPickedUp();
        OnObjectPickedUp?.Invoke(item);
        return true;
    }

    private void Drop()
    {
        Pickupable item = held;
        held = null;
        item.OnReleased();
        OnObjectReleased?.Invoke(item);
    }

    private void Throw(float chargeNormalized)
    {
        Pickupable item = held;
        held = null;

        Transform reference = cameraTransform != null ? cameraTransform : transform;
        float powerRating = Mathf.Lerp(minThrowPower, maxThrowPower, chargeNormalized);
        float impulse = powerRating * powerToImpulse;
        Vector3 direction = reference.forward + Vector3.up * 0.08f;

        item.Throw(direction, impulse, powerRating);
        OnObjectReleased?.Invoke(item);
    }

    private void NotifyChargeChanged(float normalized)
    {
        OnChargeChanged?.Invoke(normalized);
        OnChargeChangedEvent?.Invoke(normalized);
    }
}
