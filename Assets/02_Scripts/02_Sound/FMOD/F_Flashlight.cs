using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class F_Flashlight : MonoBehaviour {
    const string EVENT_PATH = "event:/Items/Lamp";

    public void PlayOnEvent() {
        EventInstance On = RuntimeManager.CreateInstance(EVENT_PATH);
        RuntimeManager.AttachInstanceToGameObject(On, transform, true);


        On.setParameterByName("WalkRun", 0, false); // 0 is walk

        On.start();
        On.release();
    }
}
