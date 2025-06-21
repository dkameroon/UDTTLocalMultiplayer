using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NetworkTransform))]
public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    private CharacterController controller;
    private CameraFollow cameraFollow;

    private Vector3 latestMoveInput;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Vector3 randomSpawnPos = new Vector3(
                Random.Range(10f, -5f),
                1f,
                Random.Range(-35f, -45f));
            transform.position = randomSpawnPos;
        }

        if (!IsOwner) return;

        cameraFollow = Camera.main.GetComponent<CameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.SetTarget(transform);
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
            ServerMoveUpdate();
        }
    }

    private void GatherAndSendInput()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null) return;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 forward = mainCam.transform.forward;
        Vector3 right = mainCam.transform.right;

        forward.y = 0;
        right.y = 0;
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

    private void ServerMoveUpdate()
    {
        if (latestMoveInput == Vector3.zero) return;

        float moveDistance = moveSpeed * Time.deltaTime;

        float playerRadius = 0.7f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, latestMoveInput, moveDistance);

        if (!canMove)
        {
            Vector3 moveDirX = new Vector3(latestMoveInput.x, 0, 0).normalized;
            canMove = latestMoveInput.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);
            if (canMove)
            {
                latestMoveInput = moveDirX;
            }
            else
            {
                Vector3 moveDirZ = new Vector3(0, 0, latestMoveInput.z).normalized;
                canMove = latestMoveInput.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);
                if (canMove)
                {
                    latestMoveInput = moveDirZ;
                }
            }
        }

        if (canMove)
        {
            controller.Move(latestMoveInput * moveDistance);
            transform.forward = Vector3.Slerp(transform.forward, latestMoveInput, Time.deltaTime * rotationSpeed);
        }
    }

    public bool IsWalking()
    {
        return latestMoveInput != Vector3.zero;
    }
}
