using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BrushHover : MonoBehaviour
{
    public Color hoverColor = new Color(1f, 0.5f, 0.3f, 1f);
    private Renderer rend;
    private Color originalColor;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend.material.HasProperty("_Color"))
        {
            originalColor = rend.material.color;
        }
    }

    public void OnHoverEnter(HoverEnterEventArgs args)
    {
        if (rend.material.HasProperty("_Color"))
            rend.material.color = hoverColor;
    }

    public void OnHoverExit(HoverExitEventArgs args)
    {
        if (rend.material.HasProperty("_Color"))
            rend.material.color = originalColor;
    }
}