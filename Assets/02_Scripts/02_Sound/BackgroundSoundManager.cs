using UnityEngine;

public class BackgroundSoundManager : MonoBehaviour{
    public static BackgroundSoundManager instance;

    AudioSource currentMusic;
    AudioSource currentAmbient;

    void Awake() { //this makes sure that the background sounds continues playing between scenes
        if (instance == null) {
            DontDestroyOnLoad(gameObject);
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }
}
