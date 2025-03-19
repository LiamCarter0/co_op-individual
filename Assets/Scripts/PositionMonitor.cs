using UnityEngine;

public class PositionMonitor : MonoBehaviour
{
    [Tooltip("The y-position threshold. If the object's y is below this, it will be adjusted.")]
    public float yThreshold = 0f;

    [Tooltip("How much to move the object up when it goes below the threshold.")]
    public float raiseAmount = 1f;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("No Rigidbody2D component found on " + gameObject.name);
        }
    }

    private void FixedUpdate()
    {
        // Check if the object's y-position is below the threshold.
        if (transform.position.y < yThreshold)
        {
            // Raise the object by the raiseAmount.
            Vector2 newPos = new Vector2(transform.position.x, transform.position.y + raiseAmount);
            transform.position = newPos;

            // Reset the velocity to zero.
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }
}
