using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    [Header("Ruta")]
    public Transform[] waypoints;
    public bool loop = true;
    public bool faceDirection = true;

    [Header("Movimiento")]
    public float speed = 3f;
    public float arrivalDistance = 0.8f;
    public float turnSpeed = 8f;

    private int index;
    private bool forward = true;

    private void Update()
    {
        if (waypoints == null || waypoints.Length == 0)
            return;

        Transform target = waypoints[index];
        if (target == null)
        {
            Advance();
            return;
        }

        Vector3 to = target.position - transform.position;
        to.y = 0f;

        if (to.sqrMagnitude <= arrivalDistance * arrivalDistance)
        {
            Advance();
            return;
        }

        Vector3 dir = to.normalized;
        transform.position += dir * speed * Time.deltaTime;

        if (faceDirection)
        {
            Quaternion look = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, turnSpeed * Time.deltaTime);
        }
    }

    private void Advance()
    {
        if (loop)
        {
            index = (index + 1) % waypoints.Length;
            return;
        }

        if (forward)
        {
            index++;
            if (index >= waypoints.Length)
            {
                index = Mathf.Max(0, waypoints.Length - 2);
                forward = false;
            }
        }
        else
        {
            index--;
            if (index < 0)
            {
                index = Mathf.Min(1, waypoints.Length - 1);
                forward = true;
            }
        }
    }
}
