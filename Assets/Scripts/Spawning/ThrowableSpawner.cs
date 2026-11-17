using System.Collections;
using UnityEngine;

public class ThrowableSpawner : MonoBehaviour
{
    [SerializeField] private GameObject throwablePrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float respawnDelay = 2f;
    [SerializeField] private float fallOffHeight = -15f;
    [SerializeField] private bool spawnOnStart = true;

    private Pickupable current;
    private Coroutine respawnRoutine;

    public Pickupable Current => current;

    private void Start()
    {
        if (spawnOnStart)
            Spawn();
    }

    private void Update()
    {
        if (current != null && current.transform.position.y < fallOffHeight)
            Destroy(current.gameObject);
    }

    private void OnDestroy()
    {
        if (current != null)
            current.Destroyed -= HandleDestroyed;
    }

    private void Spawn()
    {
        if (throwablePrefab == null)
            return;

        Transform point = spawnPoint != null ? spawnPoint : transform;
        GameObject instance = Instantiate(throwablePrefab, point.position, point.rotation);
        current = instance.GetComponentInChildren<Pickupable>();

        if (current != null)
            current.Destroyed += HandleDestroyed;
    }

    private void HandleDestroyed(Pickupable destroyed)
    {
        destroyed.Destroyed -= HandleDestroyed;

        if (current == destroyed)
            current = null;

        ScheduleRespawn();
    }

    private void ScheduleRespawn()
    {
        if (respawnRoutine != null)
            StopCoroutine(respawnRoutine);

        respawnRoutine = StartCoroutine(RespawnAfterDelay());
    }

    private IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);
        respawnRoutine = null;
        Spawn();
    }
}
