using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {
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

    CharacterController controller;
    Transform cam;
    Vector3 velocity;
    float xRotation = 0f;
    bool isGrounded;

    // Sprint variables
    bool isSprinting;
    float currentSpeed;

    // Stamina variables
    float currentStamina;
    bool isExhausted = false;
    float exhaustionTimer = 0f;

    // Footsteps
    [Header("Footsteps")]
    [SerializeField] float walkFootstepTimerLimit = 0.6f;
    [SerializeField] float runFootstepTimerLimit = 0.3f;
    [SerializeField] int walkNoiseLevel = 3;
    [SerializeField] int runNoiseLevel = 5;
    float walkFootstepTimer;
    float runFootstepTimer;

    // --- ADDED ---
    CameraShake cameraShake;
    // --------------

    void Start() {
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;

        // --- ADDED ---
        cameraShake = cam.GetComponent<CameraShake>();
        // --------------

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentSpeed = speed;
        currentStamina = maxSprintStamina;

        // (Sound)baseline idle breathing
    }

    void Update() {
        HandleMouseLook();
        HandleMovement();
        HandleStamina();
    }

    void HandleMovement() {
        #region WalkRun

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        bool wantsToSprint = Input.GetKey(KeyCode.LeftShift) && z > 0f && x == 0f;
        bool wasSprinting = isSprinting;
        isSprinting = wantsToSprint && !isExhausted && currentStamina > 0f;

        float targetSpeed = isSprinting ? speed * sprintMultiplier : speed;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, sprintAcceleration * Time.deltaTime);

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        #endregion
        #region Footsteps

        if (move != Vector3.zero && !isSprinting)  walkFootstepTimer += Time.deltaTime;
        else if (move != Vector3.zero && isSprinting)  runFootstepTimer += Time.deltaTime;
        
        if(walkFootstepTimer > walkFootstepTimerLimit) {
            walkFootstepTimer = 0;
            
            EventManager.Trigger("sfx-walk");
            EventManager.Trigger("made-noise", walkNoiseLevel);
        } 
        if(runFootstepTimer > runFootstepTimerLimit) {
            runFootstepTimer = 0;
            EventManager.Trigger("sfx-run");
            EventManager.Trigger("made-noise", runNoiseLevel);
        }

        #endregion
        #region Jump
        isGrounded = CheckGrounded();

        if (isGrounded && velocity.y < 0f && GetDistanceToGround() <= groundProximityThreshold) {
            velocity.y = -2f;

            // --- ADDED ---
            if (cameraShake != null)
                cameraShake.Shake(0.15f, 0.1f); // small landing shake
            // --------------

            // (Sound)footstep landing sound when hitting ground after jump
        }

        if (Input.GetButtonDown("Jump") && isGrounded) {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            // --- ADDED ---
            if (cameraShake != null)
                cameraShake.Shake(0.1f, 0.08f); // subtle jump shake
            // --------------

            // (Sound)jump sound
        }

        if (velocity.y < 0f)
            velocity.y += gravity * fallMultiplier * Time.deltaTime;
        else
            velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
        #endregion

        // (Sound)handle footsteps
        // -looping walking footstep sounds when moving and grounded
        // -increased footstep frequency and volume when sprinting
        // -mute footsteps when airborne
        // -possibly add heavy breathing overlay while sprinting?
        // -could add short “panting exhale” when stopping sprint suddenly (yes it does that should you ignore the warnings)
    
        if (Input.GetKeyDown(KeyCode.K)) {
    if (cameraShake != null) {
        cameraShake.Shake(0.6f, 0.5f);  // noticeable duration & magnitude
        Debug.Log("CameraShake triggered!");
    }
    else Debug.LogWarning("cameraShake reference is null");
}
    }

    void HandleMouseLook() {
        if (!Application.isFocused) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * sensitivityMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * sensitivityMultiplier;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleStamina() {
        bool wasExhausted = isExhausted;

        if (isSprinting) {
            currentStamina -= staminaDrainRate * Time.deltaTime;

            if (currentStamina <= 0f) {
                currentStamina = 0f;
                isExhausted = true;
                exhaustionTimer = exhaustionCooldown;

                // (Sound)out of breath
                // (Sound)footsteps should stop or slow to normal walking pace (this is where your sprinting gets blocked)
            } else {
                // (Sound)gradually crossfade normal breathing to heavier breathing when sprinting
                // (Sound)intensify footsteps slightly while sprinting
            }
        } else {
            if (isExhausted) {
                exhaustionTimer -= Time.deltaTime;
                if (exhaustionTimer <= 0f) {
                    isExhausted = false;
                    // (Sound)some recovery sign
                }
            } else {
                currentStamina = Mathf.Min(maxSprintStamina, currentStamina + staminaRegenRate * Time.deltaTime);

                // (Sound)gradual return to normal breathing pace as stamina refills
            }
        }
    }

    bool CheckGrounded() {
        float footHeight = controller.height / 2f - controller.radius;
        Vector3 footPos = transform.position + Vector3.down * footHeight;

        if (Physics.Raycast(footPos + Vector3.up * groundCheckOffset, Vector3.down, out RaycastHit hit, groundCheckOffset + 0.1f, groundMask))
            return true;

        if (Physics.CheckSphere(footPos + Vector3.up * groundCheckOffset, groundCheckRadius, groundMask))
            return true;

        return false;
    }

    float GetDistanceToGround() {
        RaycastHit hit;
        float footHeight = controller.height / 2f - controller.radius;
        Vector3 footPos = transform.position + Vector3.down * footHeight;

        if (Physics.Raycast(footPos, Vector3.down, out hit, 5f, groundMask))
            return hit.distance;
        else
            return Mathf.Infinity;
    }
}