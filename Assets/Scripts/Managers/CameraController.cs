using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 defaultTargetPosition = Vector3.zero;  // Default position to orbit around
    public float defaultDistance = 20f;
    public float defaultPitch = 45f;
    public float defaultYaw = 0f;

    public float distance;
    public Vector3 targetPosition;

    public float zoomSpeed = 10f;
    public float rotationSpeed = 150f;  // Increased sensitivity
    public float panSpeed = 10f;
    public float moveSpeed = 10f;
    public int edgeScrollSize = 10;     // Pixels from screen edge to trigger scroll

    private float currentYaw;
    private float currentPitch;

    private Vector3 lastInput = Vector3.zero;

    void Start()
    {
        defaultTargetPosition = targetPosition;
        // Initialize to defaults on start
        ResetCamera();
    }

    void Update()
    {
        HandleResetInput();
        HandleZoom();
        HandleRotation();
        HandleMovement();
        UpdateCameraPosition();
    }

    private void HandleResetInput()
    {
        if (Input.GetKeyDown(KeyCode.Home))
        {
            ResetCamera();
        }
    }

    private void ResetCamera()
    {
        targetPosition = defaultTargetPosition;
        distance = defaultDistance;
        currentPitch = defaultPitch;
        currentYaw = defaultYaw;
        lastInput = Vector3.zero;
    }

    private void HandleZoom()
    {
        distance -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        distance = Mathf.Clamp(distance, 10f, 50f);
    }

    private void HandleRotation()
    {
        if (Input.GetMouseButton(1) && !(Input.GetMouseButton(0))) // Right mouse button only
        {
            currentYaw += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            currentPitch -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            currentPitch = Mathf.Clamp(currentPitch, 10f, 80f);
        }
    }

    private void HandleMovement()
    {
        Vector3 input = Vector3.zero;

        // Keyboard input
        bool w = Input.GetKey(KeyCode.W);
        bool s = Input.GetKey(KeyCode.S);
        bool a = Input.GetKey(KeyCode.A);
        bool d = Input.GetKey(KeyCode.D);

        // Vertical axis
        if (w && !s)
            input.z = 1;
        else if (!w && s)
            input.z = -1;
        else if (w && s)
            input.z = lastInput.z;

        // Horizontal axis
        if (a && !d)
            input.x = -1;
        else if (!a && d)
            input.x = 1;
        else if (a && d)
            input.x = lastInput.x;

        // Edge scrolling overrides keyboard input
        Vector3 mousePos = Input.mousePosition;
        if (mousePos.x >= 0 && mousePos.x < edgeScrollSize) input.x = -1;
        else if (mousePos.x <= Screen.width && mousePos.x > Screen.width - edgeScrollSize) input.x = 1;
        if (mousePos.y >= 0 && mousePos.y < edgeScrollSize) input.z = -1;
        else if (mousePos.y <= Screen.height && mousePos.y > Screen.height - edgeScrollSize) input.z = 1;

        if (input != Vector3.zero)
        {
            input.Normalize();

            Vector3 right = transform.right;
            right.y = 0;
            right.Normalize();

            Vector3 forward = Vector3.Cross(right, Vector3.up);

            targetPosition += (right * input.x + forward * input.z) * moveSpeed * Time.deltaTime;

            lastInput = input;
        }

        // Pan camera when both left and right mouse buttons held down
        if (Input.GetMouseButton(0) && Input.GetMouseButton(1))
        {
            float panX = -Input.GetAxis("Mouse X") * panSpeed * Time.deltaTime;
            float panZ = -Input.GetAxis("Mouse Y") * panSpeed * Time.deltaTime;

            Vector3 right = transform.right;
            right.y = 0;
            right.Normalize();

            Vector3 forward = Vector3.Cross(right, Vector3.up);

            targetPosition += right * panX + forward * panZ;
        }
    }

    private void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0);
        Vector3 offset = rotation * new Vector3(0, 0, -distance);

        transform.position = targetPosition + offset;
        transform.rotation = rotation;
    }
}
