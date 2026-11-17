using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController))]
public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private float fallOffHeight = -15f;
    [SerializeField] private Transform respawnPoint;

    public UnityEvent OnRespawnedEvent = new UnityEvent();

    private CharacterController controller;
    private Vector3 respawnPosition;

    public Vector3 RespawnPosition => respawnPosition;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        respawnPosition = respawnPoint != null ? respawnPoint.position : transform.position;
    }

    private void Update()
    {
        if (transform.position.y < fallOffHeight)
            Respawn();
    }

    public void SetCheckpoint(Vector3 position)
    {
        respawnPosition = position;
    }

    public void SetCheckpoint(Transform point)
    {
        if (point != null)
            respawnPosition = point.position;
    }

    public void Respawn()
    {
        bool wasEnabled = controller.enabled;
        controller.enabled = false;
        transform.position = respawnPosition;
        controller.enabled = wasEnabled;
        OnRespawnedEvent?.Invoke();
    }
}
