using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BookHover : MonoBehaviour
{
    public Color hoverColor = new Color(1f, 0.3f, 0.3f, 1f); // 泛红颜色
    private Color originalColor;
    private Renderer rend;
    private PageFlipController pageController;
    private bool isHovered = false;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend.material.HasProperty("_Color"))
        {
            originalColor = rend.material.color;
        }
        pageController = GetComponent<PageFlipController>();
    }

    void Update()
    {
        if (rend.material.HasProperty("_Color"))
        {
            // ⭐ 如果正在翻页，就保持原色
            bool flipping = pageController != null && pageController.IsFlipping();
            Color targetColor = (!flipping && isHovered) ? hoverColor : originalColor;
            rend.material.color = Color.Lerp(rend.material.color, targetColor, Time.deltaTime * 6f);
        }
    }

    public void OnHoverEnter(HoverEnterEventArgs args)
    {
        isHovered = true;
    }

    public void OnHoverExit(HoverExitEventArgs args)
    {
        isHovered = false;
    }

    public void OnSelectEnter(SelectEnterEventArgs args)
    {
        Debug.Log("Select Entered!");
        if (pageController != null)
        {
            Debug.Log("PageFlipController found, flipping!");
            pageController.ToggleFlip();
        }
        else
        {
            Debug.LogWarning("pageController is NULL!");
        }
    }
}