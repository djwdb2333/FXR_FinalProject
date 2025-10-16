using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PaperPiece : MonoBehaviour
{
    public Transform target;
    public GameObject ghost;
    public float snapDistance = 0.15f;
    public float snapSpeed = 8f;
    public bool lockAfterSnap = true;

    [Header("Glow Feedback")]
    public Renderer ghostRenderer;
    public Color baseColor = new Color(1f, 1f, 1f, 0.3f);
    public Color highlightColor = new Color(1f, 1f, 0.6f, 0.7f);
    public float pulseSpeed = 2f;

    private XRGrabInteractable grab;
    private Rigidbody rb;
    private bool isSnapping = false;
    private bool isNearTarget = false;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        if (grab != null)
        {
            grab.selectEntered.AddListener(OnSelectEntered);
            grab.selectExited.AddListener(OnSelectExited);
        }

        if (ghost != null)
            ghost.SetActive(false);

        if (ghostRenderer != null)
            ghostRenderer.material.color = baseColor;

        Debug.Log($"[PaperPiece] Awake: initialized on {name}");
    }

    void OnDestroy()
    {
        if (grab != null)
        {
            grab.selectEntered.RemoveListener(OnSelectEntered);
            grab.selectExited.RemoveListener(OnSelectExited);
        }
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        Debug.Log($"[PaperPiece] Grabbed: {name}");
        if (ghost != null)
            ghost.SetActive(true);
        isSnapping = false;
    }

    void OnSelectExited(SelectExitEventArgs args)
    {
        Debug.Log($"[PaperPiece] Released: {name}");
        if (ghost != null)
            ghost.SetActive(false);

        if (target == null)
        {
            Debug.LogWarning("[PaperPiece] No target assigned!");
            return;
        }

        float d = Vector3.Distance(transform.position, target.position);
        Debug.Log($"[PaperPiece] Distance to target: {d:F3}");

        if (d <= snapDistance)
        {
            Debug.Log($"[PaperPiece] Within snap distance ({snapDistance}), start snapping!");
            StartSnap();
        }
        else
        {
            Debug.Log("[PaperPiece] Too far to snap.");
        }
    }

    void Update()
    {
        // 检测距离，决定是否进入高亮状态
        if (target != null && ghostRenderer != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            isNearTarget = distance < 0.4f;

            if (isNearTarget)
            {
                float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
                ghostRenderer.material.color = Color.Lerp(baseColor, highlightColor, t);
                Debug.Log($"[PaperPiece] Near target ({distance:F3}) -> glowing.");
            }
            else
            {
                ghostRenderer.material.color = baseColor;
            }
        }

        if (!isSnapping || target == null) return;

        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * snapSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.deltaTime * snapSpeed);

        if (Vector3.Distance(transform.position, target.position) < 0.002f &&
            Quaternion.Angle(transform.rotation, target.rotation) < 0.5f)
        {
            FinishSnap();
        }
    }

    void StartSnap()
    {
        isSnapping = true;
        if (rb != null) rb.isKinematic = true;
        if (grab != null) grab.enabled = false;

        Debug.Log($"[PaperPiece] StartSnap: {name}");
    }

    void FinishSnap()
    {
        isSnapping = false;
        transform.position = target.position;
        transform.rotation = target.rotation;

        if (lockAfterSnap)
        {
            if (rb != null) rb.isKinematic = true;
            if (grab != null) grab.enabled = false;
        }
        else
        {
            if (rb != null) rb.isKinematic = false;
            if (grab != null) grab.enabled = true;
        }

        Debug.Log($"[PaperPiece] ✅ Snapped into place: {name}");
    }
}