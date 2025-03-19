using UnityEngine;
//using Unity.Netcode;


public class GateTrigger : MonoBehaviour
{
    [Tooltip("The gate GameObject that will move. It must have a Rigidbody2D component.")]
    public GameObject gate;

    [Tooltip("The vertical speed at which the gate moves when triggered.")]
    public float gateVerticalSpeed = 2f;

    private Rigidbody2D gateRb;
    private int playersInTrigger = 0;

    [Tooltip("Audio clip to play when triggered.")]
    public AudioClip triggerClip;

    private AudioSource audioSource;

    private void Start()
    {
        if (gate == null)
        {
            Debug.LogError("No gate assigned to GateTrigger!");
            return;
        }

        gateRb = gate.GetComponent<Rigidbody2D>();
        if (gateRb == null)
        {
            Debug.LogError("The assigned gate does not have a Rigidbody2D component!");
        }

        // Try to get an AudioSource component. If not present, add one.
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Red_Player") || collision.CompareTag("Blue_Player"))
        {
            playersInTrigger++;
            audioSource.PlayOneShot(triggerClip);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if ((collision.CompareTag("Red_Player") && gateRb != null) || (collision.CompareTag("Blue_Player") && gateRb != null))
        {
            gateRb.linearVelocity = new Vector2(gateRb.linearVelocity.x, gateVerticalSpeed);
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Red_Player") || collision.CompareTag("Blue_Player"))
        {
            playersInTrigger--;

            // When no players remain, stop the gate's vertical movement.
            if (playersInTrigger <= 0 && gateRb != null)
            {
                gateRb.linearVelocity = new Vector2(gateRb.linearVelocity.x, -1f * gateVerticalSpeed);
            }
        }
    }
}
