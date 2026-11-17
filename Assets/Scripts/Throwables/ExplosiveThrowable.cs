using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ExplosiveThrowable : Pickupable
{
    [Header("Explosion")]
    [SerializeField] private float minimumThrowPower = 7f;
    [SerializeField] private float explosionRadius = 4f;
    [SerializeField] private float explosionForce = 800f;
    [SerializeField] private float upwardsModifier = 1.5f;
    [SerializeField] private bool destroyAfterExplosion = true;

    [Header("Events")]
    public UnityEvent OnExplodedEvent = new UnityEvent();

    public event Action<ExplosiveThrowable> Exploded;

    private bool armed;
    private bool hasExploded;

    public bool IsArmed => armed;

    public override void Throw(Vector3 direction, float impulse, float powerRating)
    {
        base.Throw(direction, impulse, powerRating);
        armed = LastThrowPower >= minimumThrowPower;
    }

    public override void OnPickedUp()
    {
        base.OnPickedUp();
        armed = false;
        hasExploded = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!armed || hasExploded || IsHeld)
            return;

        Explode();
    }

    public void Explode()
    {
        if (hasExploded)
            return;

        hasExploded = true;
        armed = false;

        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        HashSet<Rigidbody> affectedBodies = new HashSet<Rigidbody>();
        HashSet<DestructibleWall> affectedWalls = new HashSet<DestructibleWall>();

        foreach (Collider hit in hits)
        {
            Rigidbody targetBody = hit.attachedRigidbody;
            if (targetBody != null && targetBody != Body && affectedBodies.Add(targetBody))
                targetBody.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsModifier, ForceMode.Impulse);

            DestructibleWall wall = hit.GetComponentInParent<DestructibleWall>();
            if (wall != null && affectedWalls.Add(wall))
                wall.DestroyWall();
        }

        Exploded?.Invoke(this);
        OnExplodedEvent?.Invoke();

        if (destroyAfterExplosion)
            Destroy(gameObject);
    }
}
