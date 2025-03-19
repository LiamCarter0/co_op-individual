using Unity.Netcode;
using UnityEngine;
using System.Collections;



public class PlayerManager : NetworkBehaviour
{
    public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();

    [SerializeField] private GameObject playerPrefabA; // Assign in Inspector!
    [SerializeField] private GameObject playerPrefabB; // Assign in Inspector!

    [Tooltip("Assign the main camera GameObject.")]
    public GameObject mainCamera;

    [Tooltip("Audio clip to play when triggered.")]
    public AudioClip triggerClip;

    private AudioSource audioSource;

    private void Start()
    {
        // Try to get an AudioSource component. If not present, add one.
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();

        }
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log($"OnNetworkSpawn() called on {OwnerClientId}. IsServer: {IsServer}, IsClient: {IsClient}");

        if (IsServer)
        {
            // Subscribe to the server started event.
            NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
            // Also register for future client connections.
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
        else if (IsClient)
        {
            Debug.Log("Client requesting spawn from server.");
            RequestSpawnServerRpc(OwnerClientId);
        }
    }

    private void HandleServerStarted()
    {
        // Spawn the host player as soon as the server is fully up.
        SpawnHostPlayer(NetworkManager.Singleton.LocalClientId);
        // Unsubscribe if you only need to handle this once.
        NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
    }




    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Client {clientId} connected.");

        if (clientId == 0)
        {
            Debug.Log("Skipping spawn for host. Already spawned.");
            return; // Host is already handled separately
        }

        if (!NetworkManager.Singleton.ConnectedClients.ContainsKey(clientId))
        {
            Debug.LogError($"ERROR: Client {clientId} is not in the ConnectedClients dictionary.");
            return;
        }

        SpawnClientPlayer(clientId);
    }



    [ServerRpc(RequireOwnership = false)]
    public void RequestSpawnServerRpc(ulong clientId)
    {
        Debug.Log($"RequestSpawnServerRpc received from client {clientId}");

        // Ignore the host since it's not in ConnectedClients
        if (clientId == 0)
        {
            Debug.Log("Host does not need to request spawn. Ignoring.");
            return;
        }

        if (!NetworkManager.Singleton.ConnectedClients.ContainsKey(clientId))
        {
            Debug.LogError($"ERROR: Client {clientId} is not in the ConnectedClients dictionary.");
            return;
        }

        SpawnClientPlayer(clientId);
    }


    private void SpawnHostPlayer(ulong hostClientId)
    {
        if (playerPrefabA == null)
        {
            Debug.LogError("ERROR: PlayerPrefabA (Host player) is not assigned!");
            return;
        }

        Vector3 spawnPosition = new Vector3(-7.5f, -2f, 0f);
        GameObject newPlayer = Instantiate(playerPrefabA, spawnPosition, Quaternion.identity);
        NetworkObject netObj = newPlayer.GetComponent<NetworkObject>();

        if (netObj == null)
        {
            Debug.LogError("ERROR: Spawned host player prefab is missing a NetworkObject component!");
            return;
        }

        newPlayer.SetActive(true);
        netObj.SpawnAsPlayerObject(hostClientId, true);
        Debug.Log($" Host (Client 0) spawned successfully at {spawnPosition}");
    }

    private void SpawnClientPlayer(ulong clientId)
    {
        if (playerPrefabA == null || playerPrefabB == null)
        {
            Debug.LogError("ERROR: Player prefabs are not assigned in the Inspector!");
            return;
        }

        Vector3 spawnPosition = new Vector3(-7.5f, -4f, 0f);
        GameObject prefab =  playerPrefabB;

        Debug.Log($"Spawning client {clientId} at position {spawnPosition}");

        GameObject newPlayer = Instantiate(prefab, spawnPosition, Quaternion.identity);
        NetworkObject netObj = newPlayer.GetComponent<NetworkObject>();

        if (netObj == null)
        {
            Debug.LogError("ERROR: Spawned client player prefab is missing a NetworkObject component!");
            return;
        }

        newPlayer.SetActive(true);
        netObj.SpawnAsPlayerObject(clientId, true);
    }



    public override void OnNetworkDespawn()
    {
        Position.OnValueChanged -= OnStateChanged;
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    public void OnStateChanged(Vector3 previous, Vector3 current)
    {
        if (Position.Value != previous)
        {
            transform.position = Position.Value;
        }
    }

    public void Move()
    {
        if (!IsOwner) return;
        SubmitPositionRequestServerRpc(transform.position);
    }

    [ServerRpc]
    void SubmitPositionRequestServerRpc(Vector3 newPosition, ServerRpcParams rpcParams = default)
    {
        transform.position = newPosition;
        Position.Value = newPosition;
    }

    //new code for respawn

    /// <summary>
    /// Call this from your button to reset players.
    /// Also resets the camera
    /// </summary>
    public void ResetPlayers()
    {
        if (!IsServer)
        {
            // If called on a client, request the server to reset players.
            ResetPlayersServerRpc();
        }
        else
        {
            // If this is the server, perform the reset directly.
            DoResetPlayers();
        }


    }

    [ServerRpc(RequireOwnership = false)]
    private void ResetPlayersServerRpc(ServerRpcParams rpcParams = default)
    {
        DoResetPlayers();
    }

    /// <summary>
    /// Despawns current players and respawns them at their starting positions.
    /// This function must be called on the server.
    /// </summary>
    private void DoResetPlayers()
    {
        Debug.Log("Resetting players...");
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.PlayerObject != null)
            {
                // Despawn the current player object.
                client.PlayerObject.Despawn();
                // Now respawn the player using the appropriate spawn method.
                if (client.ClientId == 0)
                {
                    SpawnHostPlayer(client.ClientId);
                }
                else
                {
                    SpawnClientPlayer(client.ClientId);
                }
            }
            else
            {
                Debug.LogWarning($"Client {client.ClientId} has no player object to reset.");
            }
        }
        // Reset the main camera position using the assigned camera GameObject.
        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(0f, 0f, -10f);
            Debug.Log("Main camera reset to (0,0,-10)");
        }
        else
        {
            Debug.LogWarning("Main camera not assigned in the inspector.");
        }

        audioSource.PlayOneShot(triggerClip);

        // Activate all objects tagged with "Camera_Mover" and reset their NextScreenButton trigger.
        GameObject[] cameraMovers = GameObject.FindGameObjectsWithTag("Camera_Mover");
        foreach (GameObject mover in cameraMovers)
        {
            mover.SetActive(true);
            NextScreenButton nsb = mover.GetComponent<NextScreenButton>();
            if (nsb != null)
            {
                nsb.ResetTrigger();
            }
        }

    }
}

