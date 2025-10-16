using UnityEngine;

public class TriggerTest : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[Trigger] Entered by: {other.name}");
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log($"[Trigger] Staying with: {other.name}");
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"[Trigger] Exited by: {other.name}");
    }
}