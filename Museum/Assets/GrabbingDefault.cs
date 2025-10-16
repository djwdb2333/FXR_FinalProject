using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Video;

public class ClothGrabController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject infoPanel;      // Your text/info panel (Canvas)
    public GameObject videoPanel;     // Video panel (Quad with VideoPlayer)
    public VideoPlayer videoPlayer;   // VideoPlayer on the videoPanel
    public GameObject clothParentObject; // The cloth object to be grabbed

    [Header("VR Settings")]
    public Transform playerCamera;        // Usually your main camera
    public float videoPanelDistance = 40f; // Distance for the video panel
    public float videoPanelHeight = 1f;    // Height offset for video panel
    public float infoPanelDistance = 0.1f; // Distance of info panel from camera
    public float infoPanelRightOffset = 2.0f; // Horizontal offset to right of camera
    public float infoPanelHeightOffset = 0.2f; // Height offset from camera
    public float clothDistance = 0.5f;       // Distance of cloth from camera
    public float clothHeightOffset = 0.2f;   // Height offset of cloth from camera

    private XRGrabInteractable grabInteractable;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Camera mainCamera;
    private LayerMask originalCullingMask;
    private bool isGrabbed = false;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Save initial transform to reset later
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        mainCamera = Camera.main;
        originalCullingMask = mainCamera.cullingMask;

        // Hide panels and cloth at start
        if (infoPanel != null) infoPanel.SetActive(false);
        if (videoPanel != null) videoPanel.SetActive(false);
        if (clothParentObject != null) clothParentObject.SetActive(false);

        // Subscribe to grab/release events
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        isGrabbed = true;

        // Show only UI layer
        mainCamera.cullingMask = LayerMask.GetMask("UIOnly");

        // Video panel (far and slightly higher)
        if (videoPanel != null)
        {
            Vector3 forward = playerCamera.forward.normalized;
            Vector3 videoPos = playerCamera.position + forward * 2.0f * videoPanelDistance + Vector3.up * videoPanelHeight;
            videoPanel.transform.position = videoPos;
            videoPanel.transform.LookAt(playerCamera);
            videoPanel.transform.Rotate(0, 180, 0);
            videoPanel.SetActive(true);

            if (videoPlayer != null)
                videoPlayer.Play();
        }

        // Info panel (close, right side of camera)
        if (infoPanel != null)
        {
            Vector3 forward = playerCamera.forward.normalized;
            Vector3 right = playerCamera.right.normalized;

            Vector3 infoPos = playerCamera.position
                              + forward * infoPanelDistance
                              + right * infoPanelRightOffset
                              + Vector3.up * infoPanelHeightOffset;

            infoPanel.transform.position = infoPos;
            infoPanel.transform.LookAt(playerCamera);
            infoPanel.transform.Rotate(0, 180, 0);
            infoPanel.SetActive(true);
        }

        // Activate cloth
        if (clothParentObject != null)
            clothParentObject.SetActive(true);
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;

        // Reset cloth position/rotation
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        // Restore camera view
        mainCamera.cullingMask = originalCullingMask;

        // Hide panels and cloth
        if (infoPanel != null) infoPanel.SetActive(false);
        if (videoPanel != null) videoPanel.SetActive(false);
        if (clothParentObject != null) clothParentObject.SetActive(false);

        if (videoPlayer != null) videoPlayer.Stop();
    }

    private void LateUpdate()
    {
        if (isGrabbed && clothParentObject != null)
        {
            // Fixed position in front of camera
            Vector3 targetPos = playerCamera.position + playerCamera.forward * clothDistance + Vector3.up * clothHeightOffset;
            clothParentObject.transform.position = targetPos;

            // Optional: make it always face the camera
            clothParentObject.transform.LookAt(playerCamera);
            clothParentObject.transform.Rotate(0, 180, 0);
        }
    }
}
