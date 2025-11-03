using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 6f;
    public float gravity = -9.81f;      // Erdschwerkraft
    public float jumpHeight = 1f;       // Max Sprunghöhe in Metern

    [Header("Mouse Settings")]
    public float mouseSensitivity = 120f;

    [Header("Ground Check")]
    public LayerMask groundMask;          // Layer für Boden-Objekte
    public float groundCheckOffset = 0.05f; // kleine Höhe über Füßen
    public float groundCheckRadius = 0.45f; // kleiner als Controller-Radius
    public float groundCheckDistance = 0.15f; // Länge des SphereCasts nach unten

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
        // Bewegung
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // Bodenprüfung
        isGrounded = CheckGrounded();

        // Stabil am Boden bleiben
        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            Debug.Log("Jump triggered");
        }

        // Gravity anwenden
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    // Robuste Bodenprüfung mit SphereCast
    bool CheckGrounded()
    {
        float footHeight = controller.height / 2f - controller.radius;
        Vector3 footPos = transform.position + Vector3.down * footHeight + Vector3.up * groundCheckOffset;

        // SphereCast nach unten
        return Physics.SphereCast(footPos, groundCheckRadius, Vector3.down, out RaycastHit hit, groundCheckDistance, groundMask);
    }

    // Visualisierung der Bodenprüfung
    void OnDrawGizmosSelected()
    {
        if (controller == null) return;

        float footHeight = controller.height / 2f - controller.radius;
        Vector3 footPos = transform.position + Vector3.down * footHeight + Vector3.up * groundCheckOffset;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(footPos, groundCheckRadius);

        // Optional: Linie für SphereCast
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(footPos, footPos + Vector3.down * groundCheckDistance);
    }
}