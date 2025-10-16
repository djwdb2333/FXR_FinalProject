using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI; // For Canvas/Text

public class GunGrabber : MonoBehaviour
{
    [Header("XR Settings")]
    public XRGrabInteractable grabInteractable;
    public Camera mainCamera;

    [Header("GunScene Settings")]
    public GameObject gunSceneObjects; // Parent of all GunScene visuals
    public Canvas gunSceneCanvas;      // Canvas to show info when grabbed
    public Vector3 gunSceneOffset = new Vector3(0, 0, 1.5f);

    [Header("Animation Settings")]
    public Animator[] characterAnimators;
    public string animationTriggerName = "StartAnimation";
    public string animationStateName = "YourAnimationName";
    public float animationDelay = 0.5f;

    // Internal state
    private int originalCullingMask;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Transform originalParent;
    private Rigidbody rb;
    private XRBaseInteractor lastInteractor;

    private Vector3 gunSceneOriginalPosition;
    private Quaternion gunSceneOriginalRotation;
    private Transform gunSceneOriginalParent;

    void Start()
    {
        if (grabInteractable == null)
            grabInteractable = GetComponent<XRGrabInteractable>();

        if (mainCamera == null)
            mainCamera = Camera.main;

        rb = GetComponent<Rigidbody>();

        // Store original transforms
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalParent = transform.parent;

        if (gunSceneObjects != null)
        {
            gunSceneOriginalPosition = gunSceneObjects.transform.position;
            gunSceneOriginalRotation = gunSceneObjects.transform.rotation;
            gunSceneOriginalParent = gunSceneObjects.transform.parent;
            gunSceneObjects.SetActive(false);
        }

        // Disable canvas initially
        if (gunSceneCanvas != null)
            gunSceneCanvas.gameObject.SetActive(false);

        // Store original culling mask
        originalCullingMask = mainCamera.cullingMask;

        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    void OnGrabbed(SelectEnterEventArgs args)
    {
        lastInteractor = args.interactorObject as XRBaseInteractor;

        if (gunSceneObjects != null)
        {
            gunSceneObjects.SetActive(true);
            Vector3 targetPosition = mainCamera.transform.position
                + mainCamera.transform.forward * gunSceneOffset.z
                + mainCamera.transform.right * gunSceneOffset.x
                + mainCamera.transform.up * gunSceneOffset.y;

            gunSceneObjects.transform.position = targetPosition;
            gunSceneObjects.transform.LookAt(mainCamera.transform.position);
            gunSceneObjects.transform.Rotate(0, 180, 0);
        }

        // Show canvas info
        if (gunSceneCanvas != null)
            gunSceneCanvas.gameObject.SetActive(true);

        // Show only GunScene layer
        int gunSceneLayer = LayerMask.NameToLayer("GunScene");
        mainCamera.cullingMask = 1 << gunSceneLayer;

        StartCoroutine(TriggerAnimationsAfterDelay());
    }

    private IEnumerator TriggerAnimationsAfterDelay()
    {
        yield return new WaitForSeconds(animationDelay);

        if (characterAnimators != null && characterAnimators.Length > 0)
        {
            foreach (Animator anim in characterAnimators)
            {
                if (anim != null)
                {
                    if (!string.IsNullOrEmpty(animationTriggerName))
                        anim.SetTrigger(animationTriggerName);

                    if (!string.IsNullOrEmpty(animationStateName))
                        anim.Play(animationStateName, 0, 0f);
                }
            }
        }
    }

    void OnReleased(SelectExitEventArgs args)
    {
        StopAllCoroutines();

        // Reset animations
        if (characterAnimators != null && characterAnimators.Length > 0)
        {
            foreach (Animator anim in characterAnimators)
            {
                if (anim != null)
                {
                    if (!string.IsNullOrEmpty(animationTriggerName))
                        anim.ResetTrigger(animationTriggerName);

                    anim.Play("Idle", 0, 0f);
                }
            }
        }

        // Hide GunScene
        if (gunSceneObjects != null)
        {
            gunSceneObjects.transform.position = gunSceneOriginalPosition;
            gunSceneObjects.transform.rotation = gunSceneOriginalRotation;
            gunSceneObjects.transform.parent = gunSceneOriginalParent;
            gunSceneObjects.SetActive(false);
        }

        // Hide canvas
        if (gunSceneCanvas != null)
            gunSceneCanvas.gameObject.SetActive(false);

        // Restore camera culling mask
        int gunSceneLayer = LayerMask.NameToLayer("GunScene");
        mainCamera.cullingMask = originalCullingMask & ~(1 << gunSceneLayer);

        StartCoroutine(ResetGunAfterRelease());
    }

    private IEnumerator ResetGunAfterRelease()
    {
        yield return new WaitForEndOfFrame();

        if (grabInteractable.isSelected && lastInteractor != null)
        {
            grabInteractable.interactionManager.SelectExit(lastInteractor, grabInteractable);
        }

        transform.SetParent(originalParent);

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        transform.position = originalPosition;
        transform.rotation = originalRotation;

        yield return new WaitForEndOfFrame();

        if (rb != null)
            rb.isKinematic = false;

        lastInteractor = null;

        grabInteractable.enabled = false;
        yield return null;
        grabInteractable.enabled = true;
    }

    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }
}
