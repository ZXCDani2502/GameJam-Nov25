using System;
using UnityEngine;

public class SoundAggression : MonoBehaviour
{
    [Range(0,100)]public float aggression = 0;
    public float pacifyMultiplier = 2f;

    void OnEnable() {
        EventManager.SubscribeInteger("made-noise", AddAgression);
    }
    void OnDisable() {
        EventManager.UnsubscribeInteger("made-noise", AddAgression);
    }

    void Update() {
        aggression -= pacifyMultiplier * Time.deltaTime;
    }

    void AddAgression(int amount) => aggression += amount;
}
