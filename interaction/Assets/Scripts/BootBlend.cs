using UnityEngine;

public class BootBlend : MonoBehaviour
{
    public Renderer bootRenderer;

    private static readonly int BlendProp = Shader.PropertyToID("_Blend");
    private float blendValue = 0f;

    void Start()
    {
        if (bootRenderer == null)
            bootRenderer = GetComponent<Renderer>();

        if (bootRenderer != null)
            bootRenderer.material.SetFloat(BlendProp, 0f);
        else
            Debug.LogError("[BootBlend] Missing Renderer. Please assign bootRenderer.");
    }

    // Brush 端实时传入 0~1
    public void SetBlend(float value)
    {
        blendValue = Mathf.Clamp01(value);
        bootRenderer.material.SetFloat(BlendProp, blendValue);
        // Debug.Log($"[BootBlend] Blend set to {blendValue:F2}");
    }
}