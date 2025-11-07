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
}
