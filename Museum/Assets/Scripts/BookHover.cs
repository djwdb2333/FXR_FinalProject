using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Collider))]
public class BookHover : MonoBehaviour
{
    [Header("Visual Feedback")]
    public Color hoverColor = new Color(1f, 0.3f, 0.3f, 1f); // 悬浮时泛红

    [Header("UI Panels")]
    public GameObject hintPanel;    // 悬浮提示
    public GameObject infoPanel;    // 详情面板
    public float infoDelay = 3f;    // 点击后延迟显示秒数

    [Header("Flip")]
    public PageFlipController pageController;

    private Renderer rend;
    private Color originalColor;
    private XRBaseInteractable interactable;

    private bool isHovered = false;
    private int clickCount = 0;
    private Coroutine infoRoutine;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend != null && rend.material.HasProperty("_Color"))
            originalColor = rend.material.color;

        if (pageController == null)
            pageController = GetComponent<PageFlipController>();

        interactable = GetComponent<XRBaseInteractable>();
        if (interactable == null)
            interactable = gameObject.AddComponent<XRSimpleInteractable>();

        interactable.hoverEntered.AddListener(OnHoverEnter);
        interactable.hoverExited.AddListener(OnHoverExit);
        interactable.selectEntered.AddListener(OnSelectEnter);

        if (hintPanel) hintPanel.SetActive(false);
        if (infoPanel) infoPanel.SetActive(false);
    }

    void Update()
    {
        if (rend != null && rend.material.HasProperty("_Color"))
        {
            bool flipping = pageController != null && pageController.IsFlipping();
            Color target = (!flipping && isHovered) ? hoverColor : originalColor;
            rend.material.color = Color.Lerp(rend.material.color, target, Time.deltaTime * 6f);
        }
    }

    private void OnHoverEnter(HoverEnterEventArgs _)
    {
        isHovered = true;
        if (hintPanel) hintPanel.SetActive(true);
    }

    private void OnHoverExit(HoverExitEventArgs _)
    {
        isHovered = false;
        if (hintPanel) hintPanel.SetActive(false);
    }

    private void OnSelectEnter(SelectEnterEventArgs _)
    {
        clickCount++;
        Debug.Log($"[BookHover] Click count = {clickCount}");

        // 每次点击切换 info 状态
        bool showInfo = (clickCount % 2 == 1);

        // 先停止旧协程
        if (infoRoutine != null) StopCoroutine(infoRoutine);

        if (showInfo)
        {
            // 延迟显示 info
            infoRoutine = StartCoroutine(ShowInfoAfterDelay(infoDelay));
        }
        else
        {
            // 关闭 info
            if (infoPanel) infoPanel.SetActive(false);
        }

        // 翻页逻辑
        if (pageController != null)
            pageController.ToggleFlip();
    }

    private IEnumerator ShowInfoAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (infoPanel) infoPanel.SetActive(true);
        infoRoutine = null;
    }
}