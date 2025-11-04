using UnityEngine;

public class Flashlight : MonoBehaviour {

    Light light;

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
            EventManager.Trigger("made-noise", 6);
        }
    }
}
