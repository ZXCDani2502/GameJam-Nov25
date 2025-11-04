using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class F_Footsteps : MonoBehaviour {
    int materialValue;
    RaycastHit rayHit;
    float rayDistance = 0.3f;
    const string EVENT_PATH = "event:/Character/Footsteps";

    void OnEnable() {
        EventManager.Subscribe("sfx-walk", PlayWalkEvent);
        EventManager.Subscribe("sfx-run", PlayRunEvent);
    }
    void OnDisable() {
        EventManager.Unsubscribe("sfx-walk", PlayWalkEvent);
        EventManager.Unsubscribe("sfx-run", PlayRunEvent);
    }


    void PlayWalkEvent() {
        MaterialCheck();
        EventInstance walk = RuntimeManager.CreateInstance(EVENT_PATH);
        RuntimeManager.AttachInstanceToGameObject(walk, transform, true);


        walk.setParameterByName("Terrain", materialValue);
        walk.setParameterByName("WalkRun", 0, false); // 0 is walk

        walk.start();
        walk.release();
    }
    
    void PlayRunEvent() {
        MaterialCheck();
        EventInstance run = RuntimeManager.CreateInstance(EVENT_PATH);
        RuntimeManager.AttachInstanceToGameObject(run, transform, true);


        run.setParameterByName("Terrain", materialValue);
        run.setParameterByName("WalkRun", 1, false); //1 is run

        run.start();
        run.release();
    }

    void MaterialCheck() {
        if (Physics.Raycast(transform.position, Vector3.down, out rayHit, rayDistance, LayerMask.GetMask("Ground"))) {
            switch (rayHit.collider.tag) {
                case "Grass": materialValue = 0; break;
                case "Gravel": materialValue = 1; break;
                case "Wood": materialValue = 2; break;
            }
        }
    }
}
