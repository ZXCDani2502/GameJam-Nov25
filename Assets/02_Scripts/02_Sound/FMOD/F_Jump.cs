using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class F_Jump : MonoBehaviour {
    const string EVENT_PATH = "event:/Character/Jump";

    [SerializeField] GameObject footsteps;

    void OnEnable() {
        EventManager.Subscribe("sfx-jump", PlayJumpEvent);
        EventManager.Subscribe("sfx-land", PlayLandEvent);
    }
    void OnDisable() {
        EventManager.Unsubscribe("sfx-jump", PlayJumpEvent);
        EventManager.Unsubscribe("sfx-land", PlayLandEvent);
    }

    void PlayJumpEvent() {
        EventInstance jump = RuntimeManager.CreateInstance(EVENT_PATH);
        RuntimeManager.AttachInstanceToGameObject(jump, transform, true);

        jump.setParameterByName("Jump", 1, false); // 1 is jump

        jump.start();
        jump.release();
    }

    void PlayLandEvent() {
        EventInstance land = RuntimeManager.CreateInstance(EVENT_PATH);
        RuntimeManager.AttachInstanceToGameObject(land, footsteps.transform, true);

        land.setParameterByName("Jump", 2, false); // 2 is land

        land.start();
        land.release();

    }

}
