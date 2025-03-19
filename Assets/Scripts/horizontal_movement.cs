using UnityEngine;

public class horizontal_movement : MonoBehaviour
{

    [Tooltip("The gate GameObject that will move. It must have a Rigidbody2D component.")]
    public GameObject gate;

    

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


        // Try to get an AudioSource component. If not present, add one.
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //deleting gate because physics was taking a vacation

        gate.SetActive(false);
        audioSource.PlayOneShot(triggerClip);
    }
}
