using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathState : MonoBehaviour {

    GameObject camera;
    GameObject player;
    GameObject monster;

    void OnEnable() => EventManager.Subscribe("death-state", DeathSequence);
    void OnDisable() => EventManager.Unsubscribe("death-state", DeathSequence);

    void Start() {
        camera = GameObject.Find("Main Camera");
        player = GameObject.Find("Player");
        monster = GameObject.Find("Monster");
    }

    void DeathSequence() {
        var ct = camera.transform;
        monster.transform.position = ct.position + ct.forward;
        monster.transform.LookAt(player.transform);


        SceneManager.LoadScene("Main_Menu");
    }


    //var ct = camera.transform;
    //var mt = monster.transform;
    //monster.transform.LookAt(player.transform);
    //    for (int i = 0; i< 10; i++) {
    //        mt.position = Vector3.Lerp(mt.position, ct.position + ct.forward, i / 10);
    //        new WaitForSeconds(0.02f);

}
