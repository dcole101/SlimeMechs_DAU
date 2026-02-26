using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    public enum AxisMode { None, XOnly, YOnly, ZOnly, XY, XZ, YZ, All }

    [Header("Axis Selection")]
    public AxisMode floatAxes = AxisMode.All;

    [Header("Float Settings")]
    public float floatSpeed = 1.5f;
    public float floatHeight = 0.3f;
    public float floatSharpness = 2f;

    [Header("Rotation")]
    public bool rotateObject = true;
    public float rotationSpeed = 30f;

    private Vector3 startPosition;
    private Vector3 targetOffset;

    void Start()
    {
        startPosition = transform.localPosition;
    }

    void Update()
    {
        // Smooth floating motion using sine wave
        float time = Time.time * floatSpeed;
        Vector3 newOffset = Vector3.zero;

        if ((floatAxes == AxisMode.XOnly || floatAxes == AxisMode.XY || floatAxes == AxisMode.XZ || floatAxes == AxisMode.All))
            newOffset.x = Mathf.Sin(time) * floatHeight;
        if ((floatAxes == AxisMode.YOnly || floatAxes == AxisMode.XY || floatAxes == AxisMode.YZ || floatAxes == AxisMode.All))
            newOffset.y = Mathf.Sin(time + 2f) * floatHeight;  // Phase offset for natural motion
        if ((floatAxes == AxisMode.ZOnly || floatAxes == AxisMode.XZ || floatAxes == AxisMode.YZ || floatAxes == AxisMode.All))
            newOffset.z = Mathf.Cos(time * 0.8f) * floatHeight * 0.7f;  // Smaller, different phase

        // Smoothly interpolate to target
        targetOffset = Vector3.Lerp(targetOffset, newOffset, floatSharpness * Time.deltaTime);
        transform.localPosition = startPosition + targetOffset;

        // Gentle rotation
        if (rotateObject)
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0, Space.World);
        }
    }
}
