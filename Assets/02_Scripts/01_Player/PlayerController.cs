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
    public float groundProximityThreshold = 0.2f;

    [Header("Sprint Stamina Settings")]
    public float maxStamina = 5f;
    public float staminaDrainRate = 1f;
    public float staminaRegenRate = 0.5f;
    public float exhaustionCooldown = 4f;

    CharacterController controller;
    Transform cam;
    Vector3 velocity;
    float xRotation = 0f;
    bool isGrounded;
    bool isLanded;

    // Sprint variables
    bool isSprinting;
    float currentSpeed;

    // Stamina variables
    float currentStamina;
    bool isExhausted = false;
    float exhaustionTimer = 0f;

    [Header("Footsteps")]
    [SerializeField] float walkFootstepTimerLimit = 0.6f;
    [SerializeField] float runFootstepTimerLimit = 0.3f;
    float walkFootstepTimer;
    float runFootstepTimer;

    [Header("Noise")]
    [SerializeField] float walkNoiseLevel = 3;
    [SerializeField] float runNoiseLevel = 5;
    [SerializeField] float landNoiseLevel = 10;

    //Breathing
    bool f_walk, f_run, f_exhaust;

    CameraShake cameraShake;

    // Mouse look lock flag
    bool allowLook = true;

    // External control for PauseManager
    public void SetLookState(bool canLook) {
        allowLook = canLook;
    }

    void Start() {
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;

        cameraShake = cam.GetComponent<CameraShake>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentSpeed = speed;
        currentStamina = maxStamina;
        // (Sound)baseline idle breathing
    }

    void Update() {
    // Stoppe alles, wenn das Spiel pausiert ist
    if (Time.timeScale == 0f)
        return;

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

        if (cameraShake != null) {
            if (move.magnitude > 0.1f && isGrounded) {
                if (isSprinting) {
                    cameraShake.SetContinuousShake(0.05f, 12f);  // stronger, faster shake
                } else
                    cameraShake.SetContinuousShake(0.03f, 8f);   // gentle walk sway
            } else {
                cameraShake.StopContinuousShake();  // idle -> no sway
            }
        }

        #endregion
        #region Footsteps
        if (isGrounded) {
            if (move != Vector3.zero && !isSprinting) walkFootstepTimer += Time.deltaTime;
            else if (move != Vector3.zero && isSprinting) runFootstepTimer += Time.deltaTime;

            if (walkFootstepTimer > walkFootstepTimerLimit) {
                walkFootstepTimer = 0;

                EventManager.Trigger("sfx-walk-step");
                EventManager.Trigger("add-noise", walkNoiseLevel);
            }
            if (runFootstepTimer > runFootstepTimerLimit) {
                runFootstepTimer = 0;
                EventManager.Trigger("sfx-run-step");
                EventManager.Trigger("add-noise", runNoiseLevel);
            }
        }

        #endregion
        #region Jump
        isGrounded = CheckGrounded();

        // landing
        if (!isLanded && velocity.y < 0f && GetDistanceToGround() <= groundProximityThreshold) {
            velocity.y = -2f;
            isLanded = true;
            if (cameraShake != null)
                cameraShake.Shake(0.05f, 0.01f); // small landing shake
            EventManager.Trigger("sfx-land");
            EventManager.Trigger("add-noise", landNoiseLevel);
        }

        if (Input.GetButtonDown("Jump") && isGrounded) {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            isLanded = false;
            //if (cameraShake != null)
            //    cameraShake.Shake(0.1f, 0.08f); // subtle jump shake
            EventManager.Trigger("sfx-jump");
        }

        if (velocity.y < 0f)
            velocity.y += gravity * fallMultiplier * Time.deltaTime;
        else
            velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
        #endregion

        if (Input.GetKeyDown(KeyCode.K)) {
            if (cameraShake != null) {
                cameraShake.Shake(0.6f, 0.5f);  // noticeable duration & magnitude
                Debug.Log("CameraShake triggered!");
            } else Debug.LogWarning("cameraShake reference is null");
        }
    }

    void HandleMouseLook() {
        if (!Application.isFocused) return;

        // Stop camera when game is paused
        if (!allowLook) return;

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
            if (!f_run) {
                EventManager.Trigger("sfx-run-breath", 1 - currentStamina/maxStamina);
                f_run = true;
                f_exhaust = f_walk = false;
            }
            currentStamina -= staminaDrainRate * Time.deltaTime;

            if (currentStamina <= 0f) {
                currentStamina = 0f;
                isExhausted = true;
                exhaustionTimer = exhaustionCooldown;
            } else {
                // (Sound)gradually crossfade normal breathing to heavier breathing when sprinting
            }
        } else {
            if (isExhausted) {
                if (!f_exhaust) {
                    EventManager.Trigger("sfx-exhausted-breath");
                    f_exhaust = true;
                    f_run = f_walk = false;
                }
                exhaustionTimer -= Time.deltaTime;
                if (exhaustionTimer <= 0f) {
                    isExhausted = false;
                    // (Sound)some recovery sign
                }
            } else {
                if (!f_walk) {
                    EventManager.Trigger("sfx-walk-breath");
                    f_walk = true;
                    f_run = f_exhaust = false;
                }
                currentStamina = Mathf.Min(maxStamina, currentStamina + staminaRegenRate * Time.deltaTime);
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