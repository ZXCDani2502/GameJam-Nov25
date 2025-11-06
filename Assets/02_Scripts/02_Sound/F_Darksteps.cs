using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class F_Darksteps : MonoBehaviour {
    int materialValue;
    RaycastHit rayHit;
    float rayDistance = 0.3f;
    const string EVENT_PATH = "event:/Character/DarkSteps";

    void OnEnable() {
        EventManager.Subscribe("sfx", PlayWalkEvent);
        //EventManager.Subscribe("sfx", PlayRunEvent);
    }
    void OnDisable() {
        EventManager.Unsubscribe("sfx", PlayWalkEvent);
        //EventManager.Unsubscribe("sfx", PlayRunEvent);
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


    void MaterialCheck() {
        if (Physics.Raycast(transform.position, Vector3.down, out rayHit, rayDistance, LayerMask.GetMask("Ground"))) {
            switch (rayHit.collider.tag) {
                case "Grass": materialValue = 0; break;
                case "Gravel": materialValue = 1; break;
                case "Wood": materialValue = 2; break;
                case "Cement": materialValue = 3; break;
            }
        }
        if (Physics.Raycast(transform.position, Vector3.up, out rayHit, rayDistance, LayerMask.GetMask("Ground"))) {
            switch (rayHit.collider.tag) {
                case "Grass": materialValue = 0; break;
                case "Gravel": materialValue = 1; break;
                case "Wood": materialValue = 2; break;
                case "Cement": materialValue = 3; break;
            }
        }
    }
}
