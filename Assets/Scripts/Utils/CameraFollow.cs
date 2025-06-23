using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private float rotationSpeed = 3f;
    [SerializeField] private float distance = 35f;
    [SerializeField] private float height = 25f;
    [SerializeField] private float heightSpeed = 1f;
    [SerializeField] private float minHeight = 5f;
    [SerializeField] private float maxHeight = 25f;

    private Transform player;
    private float yaw = 0f;
    private float pitch = 20f;

    // Sets the player transform that the camera will follow.
    public void SetTarget(Transform playerTransform)
    {
        player = playerTransform;
    }

    private void Update()
    {
        if (player == null) return;

        HandleInput();
        UpdatePositionAndRotation();
    }
    
    // Handles mouse input for camera rotation and height adjustment.
    private void HandleInput()
    {
        // Rotate and adjust height only while right mouse button is held
        if (Input.GetMouseButton(1))
        {
            yaw += Input.GetAxis("Mouse X") * rotationSpeed;
            height -= Input.GetAxis("Mouse Y") * heightSpeed;
            height = Mathf.Clamp(height, minHeight, maxHeight);
        }
    }
    
    // Calculates and applies the camera position and rotation based on player position and input.
    private void UpdatePositionAndRotation()
    {
        // Direction vector relative to player
        Vector3 direction = new Vector3(0f, height, -distance);

        // Rotation from pitch and yaw angles
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        // Calculate final position relative to player
        Vector3 targetPosition = player.position + rotation * direction;

        // Set camera position and look at the player
        transform.position = targetPosition;
        transform.LookAt(player);
    }
}
