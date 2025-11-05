using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("General Settings")]
    [Tooltip("If disabled, no camera shake effects will play.")]
    [SerializeField] private bool enableShake = true;

    [Header("Impulse Shake Settings (for jumps/landings)")]
    [SerializeField, Tooltip("Default shake magnitude for short impulses.")]
    private float defaultShakeMagnitude = 0.05f;
    [SerializeField, Tooltip("How quickly an impulse shake fades away.")]
    private float dampingSpeed = 1.0f;

    [Header("Continuous Shake Settings (for walking/running)")]
    [SerializeField, Tooltip("Controls how much the camera moves while walking.")]
    private float walkShakeMagnitude = 0.03f;
    [SerializeField, Tooltip("Speed of the sway while walking.")]
    private float walkShakeSpeed = 8f;
    [SerializeField, Tooltip("Controls how much the camera moves while sprinting.")]
    private float runShakeMagnitude = 0.05f;
    [SerializeField, Tooltip("Speed of the sway while sprinting.")]
    private float runShakeSpeed = 12f;

    private Vector3 originalPos;
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.05f;

    private float continuousMagnitude = 0f;
    private float continuousSpeed = 0f;
    private float continuousTime = 0f;

    void Start()
    {
        originalPos = transform.localPosition;
    }

    void Update()
    {
        if (!enableShake)
        {
            transform.localPosition = originalPos;
            return;
        }

        continuousTime += Time.deltaTime * continuousSpeed;
        Vector3 continuousOffset = Vector3.zero;
        if (continuousMagnitude > 0f)
        {
            continuousOffset = new Vector3(
                Mathf.Sin(continuousTime) * continuousMagnitude,
                Mathf.Cos(continuousTime * 2f) * continuousMagnitude * 0.5f,
                0f
            );
        }

        Vector3 impulseOffset = Vector3.zero;
        if (shakeDuration > 0)
        {
            impulseOffset = Random.insideUnitSphere * shakeMagnitude;
            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            shakeDuration = 0f;
        }

        transform.localPosition = originalPos + impulseOffset + continuousOffset;
    }

    public void Shake(float duration, float magnitude)
    {
        if (!enableShake) return;

        shakeDuration = duration;
        shakeMagnitude = magnitude <= 0f ? defaultShakeMagnitude : magnitude;
    }

    public void SetContinuousShake(float magnitude, float speed)
    {
        if (!enableShake) return;

        continuousMagnitude = magnitude;
        continuousSpeed = speed;
    }

    public void StopContinuousShake()
    {
        continuousMagnitude = 0f;
        continuousSpeed = 0f;
    }

    public float WalkShakeMagnitude => walkShakeMagnitude;
    public float WalkShakeSpeed => walkShakeSpeed;
    public float RunShakeMagnitude => runShakeMagnitude;
    public float RunShakeSpeed => runShakeSpeed;
    public bool EnableShake => enableShake;
}