using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class F_BGM : MonoBehaviour {
    public static F_BGM instance; // Singleton

    SoundAggression monster;

    const string EVENT_PATH = "event:/Ambiance/BackGround Sound";

    [Header("Background Music")]
    float bgmTimer;
    [SerializeField] float bgmTimerLimit = 60f;
    bool bgmStarted = false;
    StudioEventEmitter bgm;


    void Awake() { //this makes sure that the background sounds continues playing between scenes
        if (instance == null) {
            DontDestroyOnLoad(gameObject);
            instance = this;
        } else {
            Destroy(gameObject);
        }
        bgm = GetComponent<StudioEventEmitter>();
    }
    void Start() => monster = GameObject.Find("Monster").GetComponent<SoundAggression>();

    void Update() {
        bgm.SetParameter("Music Amount", monster.aggression / 100f);
    }
    #region Old
    //if (bgmTimer < bgmTimerLimit) bgmTimer += Time.deltaTime;
    //else if (!bgmStarted) StartBGM();
    //else SetMusicAmount();

    //void StartBGM() {
    //    bgm = RuntimeManager.CreateInstance(EVENT_PATH);
    //    RuntimeManager.AttachInstanceToGameObject(bgm, transform, true);

    //    bgm.start();
    //    bgm.release();
    //}

    //void SetMusicAmount() {
    //    EventInstance BGM = RuntimeManager.CreateInstance(EVENT_PATH);
    //}
    #endregion
}
