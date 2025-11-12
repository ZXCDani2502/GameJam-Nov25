using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class F_Breath : MonoBehaviour {
    const string EVENT_PATH = "event:/Character/BreathWalkRun";

    EventInstance breath;

    float exhaustTimerLimit = 7f;
    float exhaustTimer = 7.8f;

    public float walkVolume = 1f;

    void OnEnable() {
        EventManager.Subscribe("sfx-walk-breath", PlayWalkEvent);
        EventManager.SubscribeFloat("sfx-run-breath", PlayRunEvent);
        EventManager.Subscribe("sfx-exhausted-breath", PlayExhaustedEvent);
    }

    void OnDisable() {
        EventManager.Unsubscribe("sfx-walk-breath", PlayWalkEvent);
        EventManager.UnsubscribeFloat("sfx-run-breath", PlayRunEvent);
        EventManager.Unsubscribe("sfx-exhausted-breath", PlayExhaustedEvent);
        breath.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    void Start() {
        breath = RuntimeManager.CreateInstance(EVENT_PATH);
        RuntimeManager.AttachInstanceToGameObject(breath, transform, true);
        PlayWalkEvent();
    }

    void Update() {
        if (exhaustTimer < exhaustTimerLimit) exhaustTimer += Time.deltaTime;
        else PlayWalkEvent();
    }

    void PlayWalkEvent() {
            Debug.Log("what");
        if (exhaustTimer > exhaustTimerLimit) { //start breathing normally only after you get your breath back
            Debug.Log("walk breath");
            breath.setVolume(walkVolume);

            breath.setParameterByName("Breathing", 0); // 0 is walk

            breath.start();
            breath.release();
        }
    }

    void PlayRunEvent(float stamina) {
        breath.setVolume(1);

        breath.setParameterByName("RunningBreath", stamina);
        breath.setParameterByName("Breathing", 1); //1 is run

        breath.start();
        breath.release();
    }

    void PlayExhaustedEvent() {
        if (exhaustTimer < exhaustTimerLimit) return;
        exhaustTimer = 0;

        breath.setParameterByName("Breathing", 2); //2 is exhausted

        breath.start();
        breath.release();
    }
}
