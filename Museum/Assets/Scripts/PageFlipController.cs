using UnityEngine;

public class PageFlipController : MonoBehaviour
{
    [Header("翻页控制")]
    public Material pageMaterial;           // 翻页材质
    public string angleProperty = "_Angle"; // Shader 中的角度属性名
    public float flipSpeed = 180f;           // 每秒角度变化速度
    public float closedAngle = 0f;           // 合上的角度
    public float openAngle = 180f;           // 打开的角度

    [Header("交互检测区")]
    public Collider hitCollider;             // 检测交互的碰撞体（推荐 BoxCollider）
    public float colliderShift = 10f;     // 翻页时检测区沿 X 轴平移距离

    private float currentAngle;              // 当前角度
    private float targetAngle;               // 目标角度
    private bool isFlipping = false;         // 是否正在翻页
    private bool isOpen = false;             // 当前是否打开状态
    private Renderer rend;
    private float lastFlipTime = 0f;         // 防抖计时
    private float debounce = 0.3f;           // 防抖间隔秒数
    private float baseCenterX = 0f;          // BoxCollider 原始中心
    private float baseLocalPosX = 0f;        // 子物体 collider 原始位置

    void Awake()
    {
        rend = GetComponent<Renderer>();

        // ⭐ 用 sharedMaterial 才能真正修改场景里的那份材质
        if (pageMaterial == null && rend != null)
        {
            pageMaterial = rend.sharedMaterial;
        }

        if (pageMaterial != null && pageMaterial.HasProperty(angleProperty))
        {
            currentAngle = pageMaterial.GetFloat(angleProperty);
        }
        else
        {
            Debug.LogWarning("PageFlipController: 材质或属性 " + angleProperty + " 未找到");
        }

        // 记录 Collider 原始位置
        if (hitCollider == null)
            hitCollider = GetComponent<Collider>();

        if (hitCollider is BoxCollider box)
        {
            baseCenterX = box.center.x;
        }
        else if (hitCollider != null && hitCollider.transform != transform)
        {
            baseLocalPosX = hitCollider.transform.localPosition.x;
        }

        targetAngle = closedAngle;
        WriteAngle(currentAngle);
    }

    void Update()
    {
        if (!isFlipping || pageMaterial == null) return;

        // 平滑推进角度
        currentAngle = Mathf.MoveTowards(currentAngle, targetAngle, flipSpeed * Time.deltaTime);
        WriteAngle(currentAngle);

        // ⭐ 平移交互检测区（Collider）而不是移动书页本身
        if (hitCollider != null)
        {
            float shiftX = Mathf.Lerp(0f, colliderShift, currentAngle / 180f);

            if (hitCollider is BoxCollider box)
            {
                var c = box.center;
                c.x = baseCenterX + shiftX;
                box.center = c;
            }
            else if (hitCollider.transform != transform)
            {
                var lp = hitCollider.transform.localPosition;
                lp.x = baseLocalPosX + shiftX;
                hitCollider.transform.localPosition = lp;
            }
        }

        if (Mathf.Approximately(currentAngle, targetAngle))
        {
            isFlipping = false;
        }
    }

    public void ToggleFlip()
    {
        if (pageMaterial == null) return;

        // 防抖：0.3 秒内不重复触发
        if (Time.time - lastFlipTime < debounce) return;
        lastFlipTime = Time.time;

        // 切换目标角度
        isOpen = !isOpen;
        targetAngle = isOpen ? openAngle : closedAngle;
        isFlipping = true;

        Debug.Log("ToggleFlip 当前角度 " + currentAngle + " 目标角度 " + targetAngle);
    }

    private void WriteAngle(float value)
    {
        if (pageMaterial != null && pageMaterial.HasProperty(angleProperty))
        {
            Renderer r = GetComponent<Renderer>();
            if (r != null)
            {
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                r.GetPropertyBlock(block);
                block.SetFloat(angleProperty, value);
                r.SetPropertyBlock(block);
            }
        }
    }

    public bool IsFlipping()
    {
        return isFlipping;
    }

    public bool IsOpen()
    {
        return isOpen;
    }

    
}