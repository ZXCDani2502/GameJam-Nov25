using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 6f;
    public float sprintMultiplier = 1.6f;          // How much faster sprinting is
    public float sprintAcceleration = 8f;          // How quickly sprinting speed ramps up
    public float gravity = -9.81f;
    public float jumpHeight = 2f;
    public float fallMultiplier = 5f;

    [Header("Mouse Settings")]
    public float mouseSensitivity = 120f;
    public float sensitivityMultiplier = 0.01f;

    [Header("Ground Check")]
    public LayerMask groundMask;
    public float groundCheckOffset = 0.02f;        // Small offset for realistic ground detection
    public float groundCheckRadius = 0.45f;
    public float groundProximityThreshold = 0.0f;  // Threshold distance to clamp velocity near ground (makes jumps not work so we set it to 0 for now)

    private CharacterController controller;
    private Transform cam;
    private Vector3 velocity;
    private float xRotation = 0f;
    private bool isGrounded;

    // Sprint-related variables
    private bool isSprinting;
    private float currentSpeed;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentSpeed = speed;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }

    void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Sprinting only when holding Shift AND moving forward (W only)
        isSprinting = Input.GetKey(KeyCode.LeftShift) && z > 0f && x == 0f;

        // Target movement speed
        float targetSpeed = isSprinting ? speed * sprintMultiplier : speed;

        // Smooth acceleration / deceleration
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, sprintAcceleration * Time.deltaTime);

        // Move the player
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Check if grounded
        isGrounded = CheckGrounded();

        // Keep player stable when grounded and close to surface
        if (isGrounded && velocity.y < 0f && GetDistanceToGround() <= groundProximityThreshold)
            velocity.y = -2f;

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // Apply gravity
        if (velocity.y < 0f)
            velocity.y += gravity * fallMultiplier * Time.deltaTime;
        else
            velocity.y += gravity * Time.deltaTime;

        // Apply vertical motion
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleMouseLook()
    {
        if (!Application.isFocused) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * sensitivityMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * sensitivityMultiplier;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    bool CheckGrounded()
    {
        float footHeight = controller.height / 2f - controller.radius;
        Vector3 footPos = transform.position + Vector3.down * footHeight;

        if (Physics.Raycast(footPos + Vector3.up * groundCheckOffset, Vector3.down, out RaycastHit hit, groundCheckOffset + 0.1f, groundMask))
            return true;

        if (Physics.CheckSphere(footPos + Vector3.up * groundCheckOffset, groundCheckRadius, groundMask))
            return true;

        return false;
    }

    float GetDistanceToGround()
    {
        RaycastHit hit;
        float footHeight = controller.height / 2f - controller.radius;
        Vector3 footPos = transform.position + Vector3.down * footHeight;

        if (Physics.Raycast(footPos, Vector3.down, out hit, 5f, groundMask))
            return hit.distance;
        else
            return Mathf.Infinity;
    }
}