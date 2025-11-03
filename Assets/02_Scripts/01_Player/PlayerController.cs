using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 6f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;
    public float fallMultiplier = 5f;

    [Header("Mouse Settings")]
    public float mouseSensitivity = 120f;
    public float sensitivityMultiplier = 0.01f;

    [Header("Ground Check")]
    public LayerMask groundMask;
    public float groundCheckOffset = 0.02f; // kleiner, realistischere Bodenerkennung
    public float groundCheckRadius = 0.45f;
    public float groundProximityThreshold = 0.1f; // Abstand zum Boden, um velocity zu clampen

    private CharacterController controller;
    private Transform cam;
    private Vector3 velocity;
    private float xRotation = 0f;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        isGrounded = CheckGrounded();

        // Boden-Stabilisierung nur, wenn wirklich sehr nah am Boden
        if (isGrounded && velocity.y < 0f && GetDistanceToGround() <= groundProximityThreshold)
            velocity.y = -2f;

        // Sprung starten
        if (Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // Gravitation anwenden
        if (velocity.y < 0f)
            velocity.y += gravity * fallMultiplier * Time.deltaTime;
        else
            velocity.y += gravity * Time.deltaTime;

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

    void OnDrawGizmosSelected()
    {
        if (controller == null) return;

        float footHeight = controller.height / 2f - controller.radius;
        Vector3 footPos = transform.position + Vector3.down * footHeight + Vector3.up * groundCheckOffset;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(footPos, groundCheckRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(footPos, footPos + Vector3.down * (groundCheckOffset + 0.1f));
    }
}