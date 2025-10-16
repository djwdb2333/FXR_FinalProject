using UnityEngine;

public class BrushTrigger : MonoBehaviour
{
    public float brushSpeed = 0.2f;   // 调快慢
    private BootBlend currentBoot;
    private float progress = 0f;
    private bool isBrushing = false;

    void OnTriggerEnter(Collider other)
    {
        var boot = other.GetComponent<BootBlend>();
        if (boot != null)
        {
            currentBoot = boot;
            isBrushing = true;
            // Debug.Log("Start brushing");
        }
    }

    void OnTriggerExit(Collider other)
    {
        var boot = other.GetComponent<BootBlend>();
        if (boot != null && boot == currentBoot)
        {
            isBrushing = false;
            // Debug.Log("Stop brushing");
        }
    }

    void Update()
    {
        if (isBrushing && currentBoot != null)
        {
            progress = Mathf.Clamp01(progress + Time.deltaTime * brushSpeed);
            currentBoot.SetBlend(progress);
        }
    }
}