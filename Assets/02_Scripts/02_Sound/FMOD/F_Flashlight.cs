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
        EventInstance On = RuntimeManager.CreateInstance(EVENT_PATH);
        RuntimeManager.AttachInstanceToGameObject(On, transform, true);


        On.setParameterByName("WalkRun", 0, false); // 0 is walk

        On.start();
        On.release();
    }
    void PlayOffEvent() {
        throw new NotImplementedException();
    }
}
