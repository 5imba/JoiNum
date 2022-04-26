using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class CanvasCameraBinder : MonoBehaviour
{
    void Awake()
    {
        Canvas canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
    }
}
