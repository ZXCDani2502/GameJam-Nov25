using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 6f;
    public float sprintMultiplier = 1.6f;
    public float sprintAcceleration = 8f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;
    public float fallMultiplier = 5f;

    [Header("Mouse Settings")]
    public float mouseSensitivity = 120f;
    public float sensitivityMultiplier = 0.01f;

    [Header("Ground Check")]
    public LayerMask groundMask;
    public float groundCheckOffset = 0.02f;
    public float groundCheckRadius = 0.45f;
    public float groundProximityThreshold = 0.0f;

    [Header("Sprint Stamina Settings")]
    public float maxSprintStamina = 5f;
    public float staminaDrainRate = 1f;
    public float staminaRegenRate = 0.5f;
    public float exhaustionCooldown = 4f;

    private CharacterController controller;
    private Transform cam;
    private Vector3 velocity;
    private float xRotation = 0f;
    private bool isGrounded;

    // Sprint variables
    private bool isSprinting;
    private float currentSpeed;

    // Stamina variables
    private float currentStamina;
    private bool isExhausted = false;
    private float exhaustionTimer = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentSpeed = speed;
        currentStamina = maxSprintStamina;

        // (Sound)baseline idle breathing
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
        HandleStamina();
    }

    void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        bool wantsToSprint = Input.GetKey(KeyCode.LeftShift) && z > 0f && x == 0f;
        bool wasSprinting = isSprinting;
        isSprinting = wantsToSprint && !isExhausted && currentStamina > 0f;

        float targetSpeed = isSprinting ? speed * sprintMultiplier : speed;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, sprintAcceleration * Time.deltaTime);

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        isGrounded = CheckGrounded();

        if (isGrounded && velocity.y < 0f && GetDistanceToGround() <= groundProximityThreshold)
        {
            velocity.y = -2f;

            // (Sound)footstep landing sound when hitting ground after jump
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            // (Sound)jump sound
        }

        if (velocity.y < 0f)
            velocity.y += gravity * fallMultiplier * Time.deltaTime;
        else
            velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        // (Sound)handle footsteps
        // -looping walking footstep sounds when moving and grounded
        // -increased footstep frequency and volume when sprinting
        // -mute footsteps when airborne
        // -possibly add heavy breathing overlay while sprinting?
        // -could add short “panting exhale” when stopping sprint suddenly (yes it does that should you ignore the warnings)
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

    void HandleStamina()
    {
        bool wasExhausted = isExhausted;

        if (isSprinting)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;

            if (currentStamina <= 0f)
            {
                currentStamina = 0f;
                isExhausted = true;
                exhaustionTimer = exhaustionCooldown;

                // (Sound)out of breath
                // (Sound)footsteps should stop or slow to normal walking pace (this is where your sprinting gets blocked)
            }
            else
            {
                // (Sound)gradually crossfade normal breathing to heavier breathing when sprinting
                // (Sound)intensify footsteps slightly while sprinting
            }
        }
        else
        {
            if (isExhausted)
            {
                exhaustionTimer -= Time.deltaTime;
                if (exhaustionTimer <= 0f)
                {
                    isExhausted = false;
                    // (Sound)some recovery sign
                }
            }
            else
            {
                currentStamina = Mathf.Min(maxSprintStamina, currentStamina + staminaRegenRate * Time.deltaTime);

                // (Sound)gradual return to normal breathing pace as stamina refills
            }
        }
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