using UnityEngine;
//https://gamedevbeginner.com/how-to-make-a-light-flicker-in-unity/

public class LightFlicker : MonoBehaviour {
    Light Light;
    public float maxInterval = 1f;
    float interval;
    float timer;

    //intensity
    public float maxFlicker = 0.2f;
    float originalIntensity;
    float targetIntensity;
    float lastIntensity;

    //color
    public float maxColorChange = 0.04f;
    Color targetColor;
    Color lastColor;
    Color originalColor;

    void Start() {
        Light = GetComponent<Light>();
        originalColor = Light.color;
        originalIntensity = Light.intensity;
    }

    void Update() {
        timer += Time.deltaTime;

        if (timer > interval) {
            timer = 0;
            interval = Random.Range(0, maxInterval);
            //intensity
            lastIntensity = Light.intensity;
            targetIntensity = Random.Range(originalIntensity - maxFlicker, originalIntensity + maxFlicker);
            //color
            lastColor = Light.color;
            targetColor = new Color(Random.Range(originalColor.r - maxColorChange, originalColor.r + maxColorChange), originalColor.g, originalColor.b);
        }
        //intensity
        Light.intensity = Mathf.Lerp(lastIntensity, targetIntensity, timer / interval);
        //color
        Light.color = Color.Lerp(lastColor, targetColor, timer / interval);
    }
}
