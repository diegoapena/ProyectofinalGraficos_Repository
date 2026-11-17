using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class VictoryZone : MonoBehaviour
{
    public UnityEvent OnVictoryEvent = new UnityEvent();

    private bool isVictory;

    public bool IsVictory => isVictory;

    private void Reset()
    {
        Collider trigger = GetComponent<Collider>();
        trigger.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isVictory || other.GetComponentInParent<PlayerRespawn>() == null)
            return;

        isVictory = true;
        OnVictoryEvent?.Invoke();
    }
}
