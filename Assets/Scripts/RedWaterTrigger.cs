using UnityEngine;

public class RedWaterTrigger : MonoBehaviour
{
    [Tooltip("Assign the panel GameObject that should be activated when Blue_Player collides.")]
    public GameObject panel;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object that entered has the tag "Blue_Player"
        if (collision.CompareTag("Blue_Player"))
        {
            if (panel != null)
            {
                //Lose sound
                audioSource.PlayOneShot(triggerClip);

                panel.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Panel not assigned in the inspector!");
            }
        }
    }
}
