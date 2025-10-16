using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HoverHint : MonoBehaviour
{
    [Header("Hint Panel")]
    public GameObject hintPanel; // 拖入 “Tap to check” 的 UI 对象

    private XRBaseInteractable interactable;

    void Awake()
    {
        interactable = GetComponent<XRBaseInteractable>();

        // 绑定事件
        interactable.hoverEntered.AddListener(OnHoverEnter);
        interactable.hoverExited.AddListener(OnHoverExit);

        // 默认隐藏
        if (hintPanel != null)
            hintPanel.SetActive(false);
    }

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        if (hintPanel != null)
            hintPanel.SetActive(true);
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        if (hintPanel != null)
            hintPanel.SetActive(false);
    }
}