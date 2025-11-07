using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.Rendering;

public class F_Monster : MonoBehaviour {
    int materialValue;
    RaycastHit rayHit;
    float rayDistance = 0.3f;
    const string EVENT_PATH1 = "event:/Character/DarkSteps";
    const string EVENT_PATH2 = "event:/Character/MonsterSteps";

    public float volume = 4;

    void OnEnable() {
        EventManager.Subscribe("sfx-follow", PlayFollowEvent);
        EventManager.Subscribe("sfx-heavy", PlayHeavyEvent);
    }

    void OnDisable() {
        EventManager.Unsubscribe("sfx-follow", PlayFollowEvent);
        EventManager.Unsubscribe("sfx-heavy", PlayHeavyEvent);
    }


    void PlayFollowEvent() {
        MaterialCheck();
        EventInstance follow = RuntimeManager.CreateInstance(EVENT_PATH1);
        RuntimeManager.AttachInstanceToGameObject(follow, transform, true);

        follow.setVolume(volume);

        follow.setParameterByName("Terrain", materialValue);
        follow.setParameterByName("WalkRun", 1, false);
        follow.start();
        follow.release();
    }
    void PlayHeavyEvent() {
        MaterialCheck();
        EventInstance heavy = RuntimeManager.CreateInstance(EVENT_PATH2);
        RuntimeManager.AttachInstanceToGameObject(heavy, transform, true);


        heavy.setParameterByName("MonsterSteps", 0, false); // 0 is walk

        heavy.start();
        heavy.release();
    }


    void MaterialCheck() {
        if (Physics.Raycast(transform.position, Vector3.down, out rayHit, rayDistance, LayerMask.GetMask("Ground"))) {
            switch (rayHit.collider.tag) {
                case "Grass": materialValue = 0; break;
                case "Gravel": materialValue = 1; break;
                case "Wood": materialValue = 2; break;
                case "Cement": materialValue = 3; break;
            }
        }
    }
}
