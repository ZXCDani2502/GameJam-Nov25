using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class F_Footsteps : MonoBehaviour {
    int materialValue;
    RaycastHit rayHit;
    float rayDistance = 0.3f;
    const string EVENT_PATH = "event:/Character/Footsteps";

    void Start() {
        //EventDescription eventDescription = RuntimeManager.GetEventDescription(EVENT_PATH);
        //eventDescription.getParameterDescriptionByName("Terrain", out paramDes);
        //TERRAIN_ID = paramDes.id;
        //Debug.Log(TERRAIN_ID.data1 + " " + TERRAIN_ID.data2);

        //TERRAIN_ID.data1 = 2021825314;
        //TERRAIN_ID.data2 = 708426747;

        //WALKRUN_ID.data1 = 1269329282;
        //WALKRUN_ID.data2 = 2170595115;
    }

    public void PlayWalkEvent() {
        MaterialCheck();
        EventInstance walk = RuntimeManager.CreateInstance(EVENT_PATH);
        RuntimeManager.AttachInstanceToGameObject(walk, transform, true);


        walk.setParameterByName("Terrain", materialValue);
        walk.setParameterByName("WalkRun", 0, false);

        walk.start();
        walk.release();
    }
    
    public void PlayRunEvent() {
        MaterialCheck();
        EventInstance walk = RuntimeManager.CreateInstance(EVENT_PATH);
        RuntimeManager.AttachInstanceToGameObject(walk, transform, true);


        walk.setParameterByName("Terrain", materialValue);
        walk.setParameterByName("WalkRun", 1, false);

        walk.start();
        walk.release();
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
