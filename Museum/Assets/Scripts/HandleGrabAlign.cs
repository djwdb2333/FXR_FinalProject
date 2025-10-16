using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSimpleInteractable))]
public class HandleGrabAlign : MonoBehaviour
{
    public Transform fakeAttach; // 拖入 Cylinder.001 下的 FakeAttach

    private XRSimpleInteractable interactable;
    private bool isSelected;

    void OnEnable()
    {
        interactable = GetComponent<XRSimpleInteractable>();
        interactable.selectEntered.AddListener(OnSelectEntered);
        interactable.selectExited.AddListener(OnSelectExited);
    }

    void OnDisable()
    {
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnSelectEntered);
            interactable.selectExited.RemoveListener(OnSelectExited);
        }
    }

    void Update()
    {
        // 选中期间每帧都对齐一次，避免物理/插值造成的微小偏移
        if (isSelected && fakeAttach != null && interactable != null)
        {
            var attach = interactable.firstInteractorSelecting?.GetAttachTransform(interactable);
            if (attach != null)
            {
                attach.position = fakeAttach.position;
                attach.rotation = fakeAttach.rotation;
            }
        }
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        isSelected = true;

        if (fakeAttach == null) { Debug.LogWarning("[HandleGrabAlign] FakeAttach not set."); return; }

        // 关键：拿到“这个交互器”对“这个可交互体”的 attach transform
        var attach = args.interactorObject.GetAttachTransform(interactable);
        if (attach != null)
        {
            attach.position = fakeAttach.position;
            attach.rotation = fakeAttach.rotation;
            Debug.Log("[HandleGrabAlign] Aligned interactor attach to FakeAttach.");
        }
        else
        {
            Debug.LogWarning("[HandleGrabAlign] Could not get attach transform.");
        }
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        isSelected = false;
    }
}