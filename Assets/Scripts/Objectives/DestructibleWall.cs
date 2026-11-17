using UnityEngine;
using UnityEngine.Events;

public class DestructibleWall : MonoBehaviour
{
    public UnityEvent OnWallDestroyedEvent = new UnityEvent();

    private bool isDestroyed;

    public bool IsDestroyed => isDestroyed;

    public void DestroyWall()
    {
        if (isDestroyed)
            return;

        isDestroyed = true;
        OnWallDestroyedEvent?.Invoke();
        Destroy(gameObject);
    }
}
