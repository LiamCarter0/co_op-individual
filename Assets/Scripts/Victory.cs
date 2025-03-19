using UnityEngine;

public class Victory : MonoBehaviour
{
    [Tooltip("Assign the victory panel GameObject that should be activated when both players are on the trigger.")]
    public GameObject victoryPanel;

    // Count the players currently inside the trigger.
    private int playersOnObject = 0;

    // Prevents multiple triggers.
    private bool hasTriggered = false;

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
        if (collision.CompareTag("Red_Player") || collision.CompareTag("Blue_Player"))
        {
            playersOnObject++;
            CheckVictory();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Red_Player") || collision.CompareTag("Blue_Player"))
        {
            playersOnObject--;
        }
    }

    private void CheckVictory()
    {
        // Trigger victory when both players are present and not already triggered.
        if (playersOnObject >= 2 && !hasTriggered)
        {
            hasTriggered = true;
            ActivateVictoryPanel();
        }
    }

    private void ActivateVictoryPanel()
    {
        //win sound
        audioSource.PlayOneShot(triggerClip);

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
            Debug.Log("Victory panel activated!");
        }
        else
        {
            Debug.LogWarning("Victory panel not assigned in the inspector!");
        }
    }
}
