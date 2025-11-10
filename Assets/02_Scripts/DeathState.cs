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



    //IEnumerator LerpMonster(Transform ct, Transform mt) {
    //    Vector3 originalPos = mt.position;
    //    for (int i = 0; i < 10; i++) {
    //        mt.position = Vector3.Lerp(originalPos, ct.position + ct.forward, i / 10);
    //        Debug.Log(mt.position);
    //        yield return new WaitForSeconds(0.1f);
    //    }
    //    SceneManager.LoadScene("Main_Menu");
    //}
}
