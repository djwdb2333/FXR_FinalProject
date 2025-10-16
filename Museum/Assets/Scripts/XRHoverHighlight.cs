using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// XRHoverHighlight
/// Changes the object's color when an XR Interactor (hand or ray)
/// hovers over it, and restores the original color when the hover ends.
/// Works with any object that has a Renderer.
/// </summary>
[RequireComponent(typeof(Renderer))]
public class XRHoverHighlight : MonoBehaviour
{
    [Header("Highlight Settings")]
    [Tooltip("Color to display while being hovered over")]
    public Color hoverColor = new Color(1f, 0.5f, 0.3f, 1f); // default orange-pink

    private Renderer rend;
    private Color originalColor;

    void Start()
    {
        // Get the Renderer component on this object
        rend = GetComponent<Renderer>();

        // If the material supports a _Color property, store its original color
        if (rend.material.HasProperty("_Color"))
        {
            originalColor = rend.material.color;
        }
        else
        {
            Debug.LogWarning($"{name} material has no _Color property. XRHoverHighlight will not work properly.");
        }
    }

    /// <summary>
    /// Called automatically when an XR Interactor starts hovering over this object.
    /// </summary>
    public void OnHoverEnter(HoverEnterEventArgs args)
    {
        if (rend.material.HasProperty("_Color"))
        {
            rend.material.color = hoverColor;
        }
    }

    /// <summary>
    /// Called automatically when an XR Interactor stops hovering over this object.
    /// </summary>
    public void OnHoverExit(HoverExitEventArgs args)
    {
        if (rend.material.HasProperty("_Color"))
        {
            rend.material.color = originalColor;
        }
    }
}