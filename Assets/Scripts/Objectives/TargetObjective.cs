using System;
using UnityEngine;
using UnityEngine.Events;

public class TargetObjective : MonoBehaviour
{
    [SerializeField] private bool destroyOnHit;
    [SerializeField] private GameObject completedVisual;

    public UnityEvent OnCompletedEvent = new UnityEvent();
    public event Action<TargetObjective> Completed;

    public bool IsCompleted { get; private set; }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsCompleted)
            return;

        Pickupable projectile = collision.collider.GetComponentInParent<Pickupable>();
        if (projectile != null && projectile.IsThrown)
            Complete();
    }

    public void Complete()
    {
        if (IsCompleted)
            return;

        IsCompleted = true;

        if (completedVisual != null)
            completedVisual.SetActive(true);

        OnCompletedEvent?.Invoke();
        Completed?.Invoke(this);

        if (destroyOnHit)
            Destroy(gameObject);
    }
}
