using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    // Server-authoritative position variable.
    public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>(
        Vector3.zero,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Initialize the position for everyone.
        if (IsServer)
        {
            Position.Value = transform.position;
        }
    }

    private void FixedUpdate()
    {
        // Update our local transform from the authoritative network position.
        transform.position = Position.Value;

        // Process input only for the owner.
        if (!IsOwner)
            return;

        // Get player input.
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveX, moveY, 0f) * moveSpeed * Time.deltaTime;

        // Instead of updating the position locally, just send the input to the server.
        if (movement != Vector3.zero)
        {
            SubmitMovementServerRpc(movement);
        }
    }

    [ServerRpc]
    private void SubmitMovementServerRpc(Vector3 movement)
    {

        // Calculate the new position using physics.
        Vector2 newPos = rb.position + new Vector2(movement.x, movement.y);

        // Move the rigidbody. This respects collisions.
        rb.MovePosition(newPos);

        // Update the network variable with the new position.
        Position.Value = new Vector3(newPos.x, newPos.y, transform.position.z);

    }
}
