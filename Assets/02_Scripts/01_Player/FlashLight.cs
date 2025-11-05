using UnityEngine;

public class Flashlight : MonoBehaviour {

    Light light;
    [SerializeField] float switchNoiseAmount = 6f;
    [SerializeField] float buzzNoiseAmount = 0.5f;

    void Awake() {
        light = GetComponent<Light>();
        light.enabled = false;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.F)) {
            if (light.enabled) {
                light.enabled = false;
                EventManager.Trigger("sfx-light-off");
            } else {
                light.enabled = true;
                EventManager.Trigger("sfx-light-on");
            }
            EventManager.Trigger("add-noise", switchNoiseAmount);
        }
        if (light.enabled) {
            EventManager.Trigger("add-noise", buzzNoiseAmount * Time.deltaTime);
        }
    }
}
