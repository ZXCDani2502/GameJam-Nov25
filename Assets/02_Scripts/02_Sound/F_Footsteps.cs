using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class F_Footsteps : MonoBehaviour {
    int materialValue;
    RaycastHit rayHit;
    float rayDistance = 0.3f;
    const string EVENT_PATH = "event:/Character/Footsteps";

    EventInstance walk;
    EventInstance run;

    void OnEnable() {
        EventManager.Subscribe("sfx-walk-step", PlayWalkEvent);
        EventManager.Subscribe("sfx-run-step", PlayRunEvent);
    }
    void OnDisable() {
        EventManager.Unsubscribe("sfx-walk-step", PlayWalkEvent);
        EventManager.Unsubscribe("sfx-run-step", PlayRunEvent);
    }


    void PlayWalkEvent() {
        MaterialCheck();

        if (walk.isValid())
            walk.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        walk = RuntimeManager.CreateInstance(EVENT_PATH);
        if (walk.isValid())
        {
            RuntimeManager.AttachInstanceToGameObject(walk, transform, true);

            walk.setParameterByName("Terrain", materialValue);
            walk.setParameterByName("WalkRun", 0, false); // 0 is walk

            walk.start();
            walk.release();

            Debug.Log("Walking");
        }
    }
    
    void PlayRunEvent() {
        MaterialCheck();

        if (run.isValid())
            run.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        run = RuntimeManager.CreateInstance(EVENT_PATH);
        if (run.isValid())
        {
            RuntimeManager.AttachInstanceToGameObject(run, transform, true);

            run.setParameterByName("Terrain", materialValue);
            run.setParameterByName("WalkRun", 1, false); //1 is run

            run.start();
            run.release();

            Debug.Log("Running");
        }
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
