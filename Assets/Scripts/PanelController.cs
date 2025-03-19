using UnityEngine;

public class PanelController : MonoBehaviour
{
    // This method will be called when the button is clicked.
    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}
