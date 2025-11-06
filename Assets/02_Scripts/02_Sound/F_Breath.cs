using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class F_Breath : MonoBehaviour {
    const string EVENT_PATH = "event:/Character/BreathWalkRun";

    void OnEnable() {
        EventManager.Subscribe("sfx-walk-breath", PlayWalkEvent);
        EventManager.SubscribeFloat("sfx-run-breath", PlayRunEvent);
        EventManager.Subscribe("sfx-exhausted-breath", PlayExhaustedEvent);
    }

    void OnDisable() {
        EventManager.Unsubscribe("sfx-walk-breath", PlayWalkEvent);
        EventManager.UnsubscribeFloat("sfx-run-breath", PlayRunEvent);
        EventManager.Unsubscribe("sfx-exhausted-breath", PlayExhaustedEvent);
    }


    void PlayWalkEvent() {
        EventInstance walk = RuntimeManager.CreateInstance(EVENT_PATH);
        RuntimeManager.AttachInstanceToGameObject(walk, transform, true);

        walk.setParameterByName("Breathing", 0); // 0 is walk

        walk.start();
        walk.release();
    }
    
    void PlayRunEvent(float stamina) {
        EventInstance run = RuntimeManager.CreateInstance(EVENT_PATH);
        RuntimeManager.AttachInstanceToGameObject(run, transform, true);

        run.setParameterByName("RunningBreath", stamina);
        run.setParameterByName("Breathing", 1); //1 is run

        run.start();
        run.release();
    }

    private void PlayExhaustedEvent() {
        EventInstance exhaust = RuntimeManager.CreateInstance(EVENT_PATH);
        RuntimeManager.AttachInstanceToGameObject(exhaust, transform, true);

        exhaust.setParameterByName("Breathing", 2); //1 is exhausted

        exhaust.start();
        exhaust.release();
    }
}
