using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

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

    [Header("Victory UI")]
    public GameObject victoryPanel;
    public TMP_Text victoryTitleText;
    public TMP_Text victoryInstructionText;
    public float victoryDelay = 5f;

    [Header("Victory Text Content")]
    [TextArea] public string victoryTitleContent = "You Won"; // editable in inspector
    [TextArea] public string victoryInstructionContent = "Press Enter to return to Main Menu"; // editable in inspector

    private bool victoryTriggered = false;

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

    // Breathing
    bool f_run, f_exhaust;

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
    }

    private void Update() {
        if (Time.timeScale == 0f)
            return;

        HandleMouseLook();
        HandleMovement();
        HandleStamina();

        // Check for Enter key to return to Main Menu after victory
        if (victoryTriggered && Input.GetKeyDown(KeyCode.Return)) {
            SceneManager.LoadScene("Main_Menu");
        }
    }

    void HandleMovement() {
        #region WalkRun
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        bool wantsToSprint = Input.GetKey(KeyCode.LeftShift) && z > 0f && x == 0f;
        isSprinting = wantsToSprint && !isExhausted && currentStamina > 0f;

        float targetSpeed = isSprinting ? speed * sprintMultiplier : speed;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, sprintAcceleration * Time.deltaTime);

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        if (cameraShake != null) {
            if (move.magnitude > 0.1f && isGrounded) {
                cameraShake.SetContinuousShake(isSprinting ? 0.05f : 0.03f, isSprinting ? 12f : 8f);
            } else {
                cameraShake.StopContinuousShake();
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

        // Landing
        if (!isLanded && velocity.y < 0f && GetDistanceToGround() <= groundProximityThreshold) {
            velocity.y = -2f;
            isLanded = true;
            cameraShake?.Shake(0.05f, 0.01f);
            EventManager.Trigger("sfx-land");
            EventManager.Trigger("add-noise", landNoiseLevel);
        }

        if (Input.GetButtonDown("Jump") && isGrounded) {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            isLanded = false;
            EventManager.Trigger("sfx-jump");
        }

        velocity.y += (velocity.y < 0f ? gravity * fallMultiplier : gravity) * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        #endregion

        if (Input.GetKeyDown(KeyCode.K)) {
            cameraShake?.Shake(0.6f, 0.5f);
        }
    }

    void HandleMouseLook() {
        if (!Application.isFocused || !allowLook) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * sensitivityMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * sensitivityMultiplier;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleStamina() {
        if (isSprinting) {
            if (!f_run) {
                EventManager.Trigger("sfx-run-breath", 1 - currentStamina / maxStamina);
                f_run = true; f_exhaust = false;
            }
            currentStamina -= staminaDrainRate * Time.deltaTime;
            if (currentStamina <= 0f) {
                currentStamina = 0f;
                isExhausted = true;
                exhaustionTimer = exhaustionCooldown;
            }
        } else {
            if (isExhausted) {
                if (!f_exhaust) {
                    EventManager.Trigger("sfx-exhausted-breath");
                    f_exhaust = true; f_run = false;
                }
                exhaustionTimer -= Time.deltaTime;
                if (exhaustionTimer <= 0f) isExhausted = false;
            } else {
                currentStamina = Mathf.Min(maxStamina, currentStamina + staminaRegenRate * Time.deltaTime);
            }
        }
    }

    bool CheckGrounded() {
        float footHeight = controller.height / 2f - controller.radius;
        Vector3 footPos = transform.position + Vector3.down * footHeight;

        return Physics.Raycast(footPos + Vector3.up * groundCheckOffset, Vector3.down, out _, groundCheckOffset + 0.1f, groundMask)
               || Physics.CheckSphere(footPos + Vector3.up * groundCheckOffset, groundCheckRadius, groundMask);
    }

    float GetDistanceToGround() {
        float footHeight = controller.height / 2f - controller.radius;
        Vector3 footPos = transform.position + Vector3.down * footHeight;

        return Physics.Raycast(footPos, Vector3.down, out RaycastHit hit, 5f, groundMask) ? hit.distance : Mathf.Infinity;
    }

    // Victory UI Integration
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Ending") && !victoryTriggered) {
            TriggerVictory();
        }
    }

    private IEnumerator ShowVictoryPanel() {
        yield return new WaitForSeconds(victoryDelay);

        if (victoryPanel != null) victoryPanel.SetActive(true);
        if (victoryTitleText != null) victoryTitleText.text = victoryTitleContent;
        if (victoryInstructionText != null) victoryInstructionText.text = victoryInstructionContent;

        SetLookState(false);
    }

    // Public methods for external control
    public void SetVictoryTitle(string title) {
        victoryTitleContent = title;
        if (victoryTitleText != null) victoryTitleText.text = title;
    }

    public void SetVictoryInstruction(string instruction) {
        victoryInstructionContent = instruction;
        if (victoryInstructionText != null) victoryInstructionText.text = instruction;
    }

    public void TriggerVictory() {
        if (!victoryTriggered) {
            victoryTriggered = true;
            StartCoroutine(ShowVictoryPanel());
        }
    }
}