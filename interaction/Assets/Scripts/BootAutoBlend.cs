using UnityEngine;

public class BootAutoBlend : MonoBehaviour
{
    public Renderer bootRenderer;   // 靴子的渲染器（拖进 Inspector）
    public float speed = 0.3f;      // 渐变速度，可调节

    private float blendValue = 0f;
    private static readonly int BlendProp = Shader.PropertyToID("_Blend");

    void Start()
    {
        // 初始化
        blendValue = 0f;
        if (bootRenderer == null)
            bootRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        // blend 从 0 慢慢增加到 1
        blendValue = Mathf.Clamp01(blendValue + Time.deltaTime * speed);

        // 更新到材质
        bootRenderer.material.SetFloat(BlendProp, blendValue);
    }
}