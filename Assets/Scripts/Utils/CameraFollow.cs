using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 3f;
    [SerializeField] private float distance = 35f;
    [SerializeField] private float height = 25f;
    [SerializeField] private float heightSpeed = 1f;
    [SerializeField] private float minHeight = 5f;
    [SerializeField] private float maxHeight = 25f;

    private Transform player;
    private float yaw = 0f;
    private float pitch = 20f;

    public void SetTarget(Transform playerTransform)
    {
        player = playerTransform;
    }

    private void Update()
    {
        if (player == null) return;

        if (Input.GetMouseButton(1))
        {
            yaw += Input.GetAxis("Mouse X") * rotationSpeed;
            height -= Input.GetAxis("Mouse Y") * heightSpeed;
            height = Mathf.Clamp(height, minHeight, maxHeight);
        }

        Vector3 direction = new Vector3(0, height, -distance);
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 targetPos = player.position + rotation * direction;

        transform.position = targetPos;
        transform.LookAt(player);
    }
}
