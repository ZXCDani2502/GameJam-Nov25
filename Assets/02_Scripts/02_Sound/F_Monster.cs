using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class F_Monster : MonoBehaviour {
    int materialValue;
    RaycastHit rayHit;
    float rayDistance = 0.3f;
    const string EVENT_PATH1 = "event:/Monster/DarkSteps";
    const string EVENT_PATH2 = "event:/Monster/MonsterSteps";
    const string EVENT_PATH3 = "event:/Monster/Excuse Me";


    public float followVolume = 4;
    public float heavyVolume = 7;

    void OnEnable() {
        EventManager.Subscribe("sfx-follow", PlayFollowEvent);
        EventManager.Subscribe("sfx-heavy", PlayHeavyEvent);
        EventManager.Subscribe("sfx-excuse", ExcuseMe);
    }

    void OnDisable() {
        EventManager.Unsubscribe("sfx-follow", PlayFollowEvent);
        EventManager.Unsubscribe("sfx-heavy", PlayHeavyEvent);
        EventManager.Unsubscribe("sfx-excuse", ExcuseMe);
    }


    void PlayFollowEvent() {
        MaterialCheck();
        EventInstance follow = RuntimeManager.CreateInstance(EVENT_PATH1);
        RuntimeManager.AttachInstanceToGameObject(follow, transform, true);

        follow.setVolume(followVolume);

        follow.setParameterByName("Terrain", materialValue);
        follow.setParameterByName("WalkRun", 1, false);
        follow.start();
        follow.release();
    }
    void PlayHeavyEvent() {
        MaterialCheck();
        EventInstance heavy = RuntimeManager.CreateInstance(EVENT_PATH2);
        RuntimeManager.AttachInstanceToGameObject(heavy, transform, true);

        heavy.setVolume(heavyVolume);

        heavy.setParameterByName("MonsterSteps", 2, false);

        heavy.start();
        heavy.release();
    }

    void ExcuseMe() {
        EventInstance excuse = RuntimeManager.CreateInstance(EVENT_PATH3);
        RuntimeManager.AttachInstanceToGameObject(excuse, transform, true);

        excuse.start();
        excuse.release();
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
