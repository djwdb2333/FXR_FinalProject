using UnityEngine;

public class BootBloodBrush : MonoBehaviour
{
    public Renderer bootRenderer;
    public Texture2D brushTexture;
    public string brushTag = "Brush"; // 记得给 brush_collider 打上这个 Tag

    private Texture2D maskTex;
    private static readonly int MaskProp = Shader.PropertyToID("_MaskTex");

    void Start()
    {
        int size = 1024;
        maskTex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] px = new Color[size * size];
        for (int i = 0; i < px.Length; i++) px[i] = Color.black;
        maskTex.SetPixels(px);
        maskTex.Apply();
        bootRenderer.material.SetTexture(MaskProp, maskTex);

        Debug.Log("[BootBloodBrush] Initialized trigger version");
    }

    // 用触发器检测刷子碰到靴子
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(brushTag))
        {
            Vector3 contactPoint = other.ClosestPoint(transform.position);
            Vector3 dir = transform.position - contactPoint;

            if (Physics.Raycast(contactPoint + dir * 0.01f, -dir, out RaycastHit hit, 0.02f))
            {
                if (hit.collider == GetComponent<Collider>())
                {
                    Vector2 uv = hit.textureCoord;
                    int px = Mathf.RoundToInt(uv.x * maskTex.width);
                    int py = Mathf.RoundToInt(uv.y * maskTex.height);
                    PaintCircle(px, py);
                    Debug.Log($"[BootBloodBrush] Painting at ({px},{py}) from trigger");
                }
            }
        }
    }

    void PaintCircle(int cx, int cy)
    {
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
    }
}