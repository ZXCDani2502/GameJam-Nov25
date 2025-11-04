using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class F_Flashlight : MonoBehaviour {
    const string EVENT_PATH = "event:/Items/Lamp";

    void OnEnable() {
        EventManager.Subscribe("sfx-light-on", PlayOnEvent);
        EventManager.Subscribe("sfx-light-off", PlayOffEvent);
    }
    void OnDisable() {
        EventManager.Unsubscribe("sfx-light-on", PlayOnEvent);
        EventManager.Unsubscribe("sfx-light-off", PlayOffEvent);
    }


    void PlayOnEvent() {
        EventInstance on = RuntimeManager.CreateInstance(EVENT_PATH);
        RuntimeManager.AttachInstanceToGameObject(on, transform, true);


        on.setParameterByName("LightSwitch", 1, false); // 1 is on

        on.start();
        on.release();
    }

    void PlayOffEvent() {
        EventInstance off = RuntimeManager.CreateInstance(EVENT_PATH);
        RuntimeManager.AttachInstanceToGameObject(off, transform, true);

        // turn off buzzing

        off.setParameterByName("LightSwitch", 3, false); // 3 is off

        off.start();
        off.release();
    }
}
