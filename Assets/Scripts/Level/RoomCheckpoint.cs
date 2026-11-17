using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class RoomCheckpoint : MonoBehaviour
{
    [SerializeField] private PlayerRespawn playerRespawn;
    [SerializeField] private DoorController previousDoor;
    [SerializeField] private DoorController nextDoor;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private bool closePreviousDoor = true;
    [SerializeField] private bool openNextDoor = true;

    public UnityEvent OnActivatedEvent = new UnityEvent();

    private bool activated;

    public bool Activated => activated;

    private void Reset()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        box.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerRespawn respawn = other.GetComponentInParent<PlayerRespawn>();

        if (respawn == null)
            return;

        if (playerRespawn == null)
            playerRespawn = respawn;

        if (respawn != playerRespawn)
            return;

        Activate();
    }

    public void Activate()
    {
        if (activated)
            return;

        activated = true;

        if (playerRespawn != null)
            playerRespawn.SetCheckpoint(respawnPoint != null ? respawnPoint.position : transform.position);

        if (closePreviousDoor && previousDoor != null)
            previousDoor.ForceClose();

        if (openNextDoor && nextDoor != null)
            nextDoor.ForceOpen();

        OnActivatedEvent?.Invoke();
    }
}
