using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Tracking Target")]
    public Transform player;

    [Header("Fluid Movement Settings")]
    [Range(0f, 1f)]
    public float smoothSpeed = 0.125f; // Lower numbers = smoother trailing catch-up lag

    [Header("Level Boundary Constraints")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    private float camHeight;
    private float camWidth;

    void Start()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            camHeight = cam.orthographicSize;
            camWidth = camHeight * cam.aspect;
        }
    }

    void LateUpdate()
    {
        if (player == null) return;

        // 1. Target coordinates where the camera wants to focus
        float targetX = player.position.x;
        float targetY = player.position.y;

        // 2. Clamp target points inside your teammate's map layout
        float clampedX = Mathf.Clamp(targetX, minX + camWidth, maxX - camWidth);
        float clampedY = Mathf.Clamp(targetY, minY + camHeight, maxY - camHeight);

        Vector3 targetPosition = new Vector3(clampedX, clampedY, transform.position.z);

        // 3. FLUID MIXING: Smoothly interpolate current position to clamped position
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
    }
}