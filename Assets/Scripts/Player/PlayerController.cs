using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NetworkTransform))]
public class PlayerController : NetworkBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    private CharacterController controller;
    private CameraFollow cameraFollow;

    // Stores the latest movement input from the owner client, synchronized via ServerRpc
    private Vector3 latestMoveInput;

    private const float PlayerRadius = 0.7f;
    private const float PlayerHeight = 2f;
    
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            SetRandomSpawnPosition();
        }

        if (IsOwner)
        {
            SetupCameraFollow();
        }
    }

    private void Update()
    {
        if (IsOwner)
        {
            GatherAndSendInput();
        }

        if (IsServer)
        {
            ProcessMovement();
        }
    }

    // Gathers input from the local player and sends it to the server.
    private void GatherAndSendInput()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null) return;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 forward = mainCam.transform.forward;
        Vector3 right = mainCam.transform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = forward * moveZ + right * moveX;
        latestMoveInput = moveDir.normalized;

        SendMoveInputServerRpc(latestMoveInput);
    }

    [ServerRpc]
    private void SendMoveInputServerRpc(Vector3 moveInput, ServerRpcParams rpcParams = default)
    {
        latestMoveInput = moveInput;
    }

    // Processes player movement on the server, including collision checks.
    private void ProcessMovement()
    {
        if (latestMoveInput == Vector3.zero) return;

        float moveDistance = moveSpeed * Time.deltaTime;

        // Check if movement in desired direction is possible without collision
        if (CanMove(latestMoveInput, moveDistance))
        {
            MovePlayer(latestMoveInput, moveDistance);
            RotatePlayerTowards(latestMoveInput);
        }
        else
        {
            // Try moving only on X axis
            Vector3 moveDirX = new Vector3(latestMoveInput.x, 0f, 0f).normalized;
            if (latestMoveInput.x != 0 && CanMove(moveDirX, moveDistance))
            {
                MovePlayer(moveDirX, moveDistance);
                RotatePlayerTowards(moveDirX);
                latestMoveInput = moveDirX;
            }
            else
            {
                // Try moving only on Z axis
                Vector3 moveDirZ = new Vector3(0f, 0f, latestMoveInput.z).normalized;
                if (latestMoveInput.z != 0 && CanMove(moveDirZ, moveDistance))
                {
                    MovePlayer(moveDirZ, moveDistance);
                    RotatePlayerTowards(moveDirZ);
                    latestMoveInput = moveDirZ;
                }
            }
        }
    }


    // Checks whether the player can move in the given direction without colliding with obstacles.
    private bool CanMove(Vector3 direction, float distance)
    {
        Vector3 start = transform.position;
        Vector3 end = start + Vector3.up * PlayerHeight;
        return !Physics.CapsuleCast(start, end, PlayerRadius, direction, distance);
    }
    
    // Moves the player in the specified direction.
    private void MovePlayer(Vector3 direction, float distance)
    {
        controller.Move(direction * distance);
    }
    
    // Smoothly rotates the player to face the movement direction.
    private void RotatePlayerTowards(Vector3 direction)
    {
        transform.forward = Vector3.Slerp(transform.forward, direction, Time.deltaTime * rotationSpeed);
    }
    

    private void SetRandomSpawnPosition()
    {
        Vector3 randomSpawnPos = new Vector3(
            Random.Range(-5f, 10f),  // Corrected range for clarity
            1f,
            Random.Range(-45f, -35f));
        transform.position = randomSpawnPos;
    }

    private void SetupCameraFollow()
    {
        cameraFollow = Camera.main?.GetComponent<CameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.SetTarget(transform);
        }
        else
        {
            Debug.LogWarning("CameraFollow component not found on Main Camera.");
        }
    }
    
    // Returns whether the player is currently moving.
    public bool IsWalking() => latestMoveInput != Vector3.zero;

}
