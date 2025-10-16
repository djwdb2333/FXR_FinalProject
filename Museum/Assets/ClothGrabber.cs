using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ClothGrabber : MonoBehaviour
{
    public XRGrabInteractable grabInteractable;
    public Camera mainCamera;
    public GameObject canvas;
    public GameObject deadHolder;

    private int originalCullingMask;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Rigidbody rb;
    private Transform originalParent;
    private XRBaseInteractor lastInteractor; // Track which controller grabbed

    public Vector3 canvasOffset = new Vector3(0, 2f, 5f);

    void Start()
    {
        if (grabInteractable == null)
            grabInteractable = GetComponent<XRGrabInteractable>();

        if (mainCamera == null)
            mainCamera = Camera.main;

        rb = GetComponent<Rigidbody>();
        originalCullingMask = mainCamera.cullingMask;

        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalParent = transform.parent;

        if (canvas != null) canvas.SetActive(false);
        if (deadHolder != null) deadHolder.SetActive(false);

        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    void OnGrabbed(SelectEnterEventArgs args)
    {
        // Store the interactor (controller) that grabbed this object
        lastInteractor = args.interactorObject as XRBaseInteractor;

        if (canvas != null) canvas.SetActive(true);
        if (deadHolder != null) deadHolder.SetActive(true);

        if (deadHolder != null)
            deadHolder.transform.position = mainCamera.transform.position + mainCamera.transform.forward * 4f;

        if (canvas != null && deadHolder != null)
            canvas.transform.position = deadHolder.transform.position + canvasOffset;

        int clothOnlyLayer = LayerMask.NameToLayer("ClothOnly");
        mainCamera.cullingMask = 1 << clothOnlyLayer;

    }

    void OnReleased(SelectExitEventArgs args)
    {
        mainCamera.cullingMask = originalCullingMask;

        if (canvas != null) canvas.SetActive(false);
        if (deadHolder != null) deadHolder.SetActive(false);

        StartCoroutine(ResetPositionAfterRelease());
    }

    private IEnumerator ResetPositionAfterRelease()
    {
        // Wait for XR Toolkit to finish its release logic
        yield return new WaitForEndOfFrame();

        // Force clear any remaining interactions
        if (lastInteractor != null && grabInteractable.isSelected)
        {
            grabInteractable.interactionManager.SelectExit(lastInteractor, grabInteractable);
        }

        // Detach from any parent
        transform.SetParent(originalParent);

        // Stop all physics
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // Reset transform
        transform.position = originalPosition;
        transform.rotation = originalRotation;

        // Wait before re-enabling physics
        yield return new WaitForEndOfFrame();

        // Re-enable physics
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        // Force refresh the interaction state
        yield return new WaitForSeconds(0.05f);

        // Clear the last interactor reference
        lastInteractor = null;

        // Force the grab interactable to update its colliders
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