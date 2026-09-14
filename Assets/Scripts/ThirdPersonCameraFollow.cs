using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class ThirdPersonCameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 3.2f, -6f);
    [SerializeField] private float followSmoothTime = 0.08f;
    [SerializeField] private float mouseSensitivity = 2.5f;
    [SerializeField] private float minPitch = -25f;
    [SerializeField] private float maxPitch = 65f;

    private Vector3 followVelocity;
    private float yaw;
    private float pitch = 18f;

    private void Start()
    {
        Vector3 euler = transform.eulerAngles;
        yaw = euler.y;
        pitch = NormalizePitch(euler.x);
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        if (IsRotatingCamera())
        {
            Vector2 mouseDelta = ReadMouseDelta();
            yaw += mouseDelta.x * mouseSensitivity * 0.05f;
            pitch -= mouseDelta.y * mouseSensitivity * 0.05f;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredPosition = target.position + rotation * offset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref followVelocity, followSmoothTime);
        transform.LookAt(target.position + Vector3.up * 1.25f);
    }

    public void SetTarget(Transform followTarget)
    {
        target = followTarget;
    }

    private static float NormalizePitch(float angle)
    {
        return angle > 180f ? angle - 360f : angle;
    }

    private static bool IsRotatingCamera()
    {
#if ENABLE_INPUT_SYSTEM
        if (Mouse.current != null)
        {
            return Mouse.current.rightButton.isPressed;
        }
#endif
        return Input.GetMouseButton(1);
    }

    private static Vector2 ReadMouseDelta()
    {
#if ENABLE_INPUT_SYSTEM
        if (Mouse.current != null)
        {
            return Mouse.current.delta.ReadValue();
        }
#endif
        return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * 20f;
    }
}
