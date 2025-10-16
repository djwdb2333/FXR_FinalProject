using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandleAutoRotate : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 120f;            // 转动速度
    public Vector3 rotationAxis = Vector3.forward; // 转动轴（根据你的场景改）
    private bool isRotating = false;

    [Header("Grab Reference")]
    public XRSimpleInteractable grabHandle; // 拖 Cylinder.001 到这里

    void Start()
    {
        if (grabHandle != null)
        {
            grabHandle.selectEntered.AddListener(_ => StartRotate());
            grabHandle.selectExited.AddListener(_ => StopRotate());
        }
        else
        {
            Debug.LogWarning($"{name}: grab handle not assigned!");
        }
    }

    void Update()
    {
        if (isRotating)
        {
            transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime, Space.Self);
        }
    }

    void StartRotate()
    {
        isRotating = true;
        Debug.Log("[HandleAutoRotate] start rotating");
    }

    void StopRotate()
    {
        isRotating = false;
        Debug.Log("[HandleAutoRotate] stop rotating");
    }
}