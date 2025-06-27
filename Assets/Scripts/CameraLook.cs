using UnityEngine;

public class CameraLook : MonoBehaviour
{
    [SerializeField] Transform player = null;
    float xRotation = 0f;
    [SerializeField] float xSensitivity = 500f, ySensitivity = 500f;

    [SerializeField] float clampAngle = 80f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        float mouseY = Input.GetAxis("Mouse Y") * ySensitivity * Time.deltaTime;
        float mouseX = Input.GetAxis("Mouse X") * xSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -clampAngle, clampAngle);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        player.Rotate(Vector3.up * mouseX);
    }
}
