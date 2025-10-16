using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BootBloodVR : MonoBehaviour
{
    public Renderer bootRenderer;      // 靴子主体渲染器
    public Texture2D brushTexture;     // 笔刷贴图
    private Texture2D maskTex;
    private static readonly int MaskProp = Shader.PropertyToID("_MaskTex");

    private bool isGrabbing = false;   // 是否正在擦拭
    private XRRayInteractor activeRay; // 当前使用的射线

    void Start()
    {
        Debug.Log("[BootBloodVR] Start() called — initializing mask texture");

        // 初始化遮罩图
        int size = 1024;
        maskTex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        var px = new Color[size * size];
        for (int i = 0; i < px.Length; i++) px[i] = Color.black;
        maskTex.SetPixels(px);
        maskTex.Apply();
        bootRenderer.material.SetTexture(MaskProp, maskTex);

        Debug.Log("[BootBloodVR] Mask texture initialized successfully");
    }

    // 当任意射线抓取靴子时
    public void OnSelectEnter(SelectEnterEventArgs args)
    {
        isGrabbing = true;
        activeRay = args.interactorObject as XRRayInteractor;
        Debug.Log("[BootBloodVR] Grab started. Interactor: " + (activeRay ? activeRay.name : "null"));
    }

    // 当松开时
    public void OnSelectExit(SelectExitEventArgs args)
    {
        isGrabbing = false;
        activeRay = null;
        Debug.Log("[BootBloodVR] Grab released.");
    }

    void Update()
    {
        if (!isGrabbing || activeRay == null)
            return;

        if (activeRay.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            Debug.Log("[BootBloodVR] Ray hit: " + hit.collider.gameObject.name);

            if (hit.collider != null && hit.collider.gameObject == bootRenderer.gameObject)
            {
                Vector2 uv = hit.textureCoord;
                int px = Mathf.RoundToInt(uv.x * maskTex.width);
                int py = Mathf.RoundToInt(uv.y * maskTex.height);
                Debug.Log($"[BootBloodVR] Painting at pixel ({px}, {py}) — UV: {uv}");
                PaintCircle(px, py);
            }
            else
            {
                Debug.Log("[BootBloodVR] Hit something else, not boot body");
            }
        }
        else
        {
            Debug.Log("[BootBloodVR] No ray hit detected");
        }
    }

    void PaintCircle(int cx, int cy)
    {
        Debug.Log($"[BootBloodVR] PaintCircle called at ({cx}, {cy})");

        int bw = brushTexture.width;
        int bh = brushTexture.height;
        int ox = cx - bw / 2;
        int oy = cy - bh / 2;

        Color[] brush = brushTexture.GetPixels();

        for (int y = 0; y < bh; y++)
        {
            int ty = oy + y;
            if (ty < 0 || ty >= maskTex.height) continue;

            for (int x = 0; x < bw; x++)
            {
                int tx = ox + x;
                if (tx < 0 || tx >= maskTex.width) continue;

                Color b = brush[y * bw + x];
                if (b.a <= 0.001f) continue;

                Color m = maskTex.GetPixel(tx, ty);
                float nv = Mathf.Clamp01(m.r + b.a);
                maskTex.SetPixel(tx, ty, new Color(nv, nv, nv, 1f));
            }
        }

        maskTex.Apply();
        Debug.Log("[BootBloodVR] Paint applied successfully");
    }
}