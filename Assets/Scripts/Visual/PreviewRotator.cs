using UnityEngine;

public class PreviewRotator : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0f, 18f, 0f);

    private void Update()
    {
        transform.Rotate(rotationSpeed * Time.unscaledDeltaTime, Space.Self);
    }
}
