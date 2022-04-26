using UnityEngine;

public class SimpleRotation : MonoBehaviour
{
    [SerializeField] private float speed = 0f;
    [SerializeField] private bool isActive = false;
    [Header("Freeze Rotation Axes")]
    [SerializeField] private bool x = false;
    [SerializeField] private bool y = false;
    [SerializeField] private bool z = false;

    private void Update()
    {
        if (isActive)
        {
            float offset = Time.deltaTime * speed;
            Vector3 eulerAngles = transform.rotation.eulerAngles;

            eulerAngles.x = x ? 0f : offset;
            eulerAngles.y = y ? 0f : offset;
            eulerAngles.z = z ? 0f : offset;

            transform.Rotate(eulerAngles, Space.Self);
        }
    }
}
