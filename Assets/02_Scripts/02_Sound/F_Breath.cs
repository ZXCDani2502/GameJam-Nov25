using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class F_Breath : MonoBehaviour {
    const string EVENT_PATH = "event:/Character/BreathWalkRun";

    EventInstance exhaust, run, walk;

    void OnEnable() {
        EventManager.Subscribe("sfx-walk-breath", PlayWalkEvent);
        EventManager.SubscribeFloat("sfx-run-breath", PlayRunEvent);
        EventManager.Subscribe("sfx-exhausted-breath", PlayExhaustedEvent);
    }

    void OnDisable() {
        EventManager.Unsubscribe("sfx-walk-breath", PlayWalkEvent);
        EventManager.UnsubscribeFloat("sfx-run-breath", PlayRunEvent);
        EventManager.Unsubscribe("sfx-exhausted-breath", PlayExhaustedEvent);
        if (exhaust.isValid()) exhaust.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        if (run.isValid()) run.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        if (walk.isValid()) walk.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }


    void PlayWalkEvent() {
        walk = RuntimeManager.CreateInstance(EVENT_PATH);
        RuntimeManager.AttachInstanceToGameObject(walk, transform, true);

        if (run.isValid()) 
            run.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT); //allows walk breath to play after running

        PLAYBACK_STATE state;
        if (exhaust.isValid())
        {
            exhaust.getPlaybackState(out state);
            if (state != PLAYBACK_STATE.PLAYING)
            { //start breathing normally only after you get your breath back
                walk.setParameterByName("Breathing", 0); // 0 is walk
                walk.start();
                walk.release();
            }
        }
    }

    void PlayRunEvent(float stamina) {
        run = RuntimeManager.CreateInstance(EVENT_PATH);
        RuntimeManager.AttachInstanceToGameObject(run, transform, true);

        if (exhaust.isValid())
            exhaust.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        if (run.isValid())
        {
            run.setParameterByName("RunningBreath", stamina);
            run.setParameterByName("Breathing", 1); //1 is run

            run.start();
            run.release();
        }
    }

    void PlayExhaustedEvent() {
        exhaust = RuntimeManager.CreateInstance(EVENT_PATH);
        if (!exhaust.isValid()) return;

        RuntimeManager.AttachInstanceToGameObject(exhaust, transform, true);

        exhaust.setParameterByName("Breathing", 2); //2 is exhausted

        exhaust.start();
        exhaust.release();
    }
}
