using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class F_Flashlight : MonoBehaviour {
    const string EVENT_PATH = "event:/Items/Lamp";
    EventInstance buzz;

    void OnEnable() {
        EventManager.Subscribe("sfx-light-on", PlayOnEvent);
        EventManager.Subscribe("sfx-light-off", PlayOffEvent);
    }
    void OnDisable() {
        EventManager.Unsubscribe("sfx-light-on", PlayOnEvent);
        EventManager.Unsubscribe("sfx-light-off", PlayOffEvent);
        if(buzz.isValid()) buzz.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }


    void PlayOnEvent() {
        EventInstance on = RuntimeManager.CreateInstance(EVENT_PATH);
        buzz = RuntimeManager.CreateInstance(EVENT_PATH);

        if (buzz.isValid())
        {
            RuntimeManager.AttachInstanceToGameObject(on, transform, true);
            RuntimeManager.AttachInstanceToGameObject(buzz, transform, true);

            buzz.setParameterByName("LightSwitch", 2, false); // 2 is buzz
            buzz.start();
            buzz.release();
        }

        if (on.isValid())
        {
            on.setParameterByName("LightSwitch", 1, false); // 1 is on
            on.start();
            on.release();
        }
    }

    void PlayOffEvent() {
        EventInstance off = RuntimeManager.CreateInstance(EVENT_PATH);
        if (off.isValid()) RuntimeManager.AttachInstanceToGameObject(off, transform, true);

        if (buzz.isValid()) buzz.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        if (off.isValid())
        {
            off.setParameterByName("LightSwitch", 3, false); // 3 is off
            off.start();
            off.release();
        }
    }
}
