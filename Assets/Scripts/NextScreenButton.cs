using UnityEngine;

public class NextScreenButton : MonoBehaviour
{
    [Tooltip("How far to move the camera vertically when this button is pressed.")]
    public float verticalMoveAmount = 2f;

    [Tooltip("Assign the camera to move. If left blank, Camera.main will be used.")]
    public Camera cameraToMove;

    // Counts the players currently inside the trigger.
    private int playersOnButton = 0;

    // Prevents multiple triggers from the same button.
    private bool hasTriggered = false;

    [Tooltip("Audio clip to play when triggered.")]
    public AudioClip triggerClip;

    private AudioSource audioSource;

    private void Start()
    {
        if (cameraToMove == null)
        {
            cameraToMove = Camera.main;
        }

        // Try to get an AudioSource component. If not present, add one.
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();

        }
    }

    // For 2D, use OnTriggerEnter2D and OnTriggerExit2D.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Try to get an AudioSource component. If not present, add one.
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();

        }

        if (collision.CompareTag("Red_Player") || collision.CompareTag("Blue_Player"))
        {
            playersOnButton++;
            CheckTrigger();
            //move sound
            audioSource.PlayOneShot(triggerClip);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Red_Player") || collision.CompareTag("Blue_Player"))
        {
            playersOnButton--;
        }
    }

    private void CheckTrigger()
    {
        // If both players are on the button and it hasn't triggered yet...
        if (playersOnButton >= 2 && !hasTriggered)
        {
            hasTriggered = true;
            MoveCameraUp();         
            
            // disable the button after use:
            gameObject.SetActive(false);
        }
    }

    private void MoveCameraUp()
    {
        if (cameraToMove != null)
        {
            

            cameraToMove.transform.position += new Vector3(0f, verticalMoveAmount, 0f);
            Debug.Log("Camera moved up by " + verticalMoveAmount);
        }
        else
        {
            Debug.LogWarning("No camera assigned to the BigButton!");
        }
    }

    public void ResetTrigger()
    {
        hasTriggered = false;
    }
}
