using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ShowHistoryOnGrab : MonoBehaviour
{
    public GameObject infoPanel; // assign your panel here in the Inspector

    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        // Get the XRGrabInteractable component attached to this object
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Subscribe to grab and release events
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);

        // Make sure the panel starts hidden
        if (infoPanel != null)
            infoPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        // Clean up listeners
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (infoPanel != null)
            infoPanel.SetActive(true);
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (infoPanel != null)
            infoPanel.SetActive(false);
    }
}
