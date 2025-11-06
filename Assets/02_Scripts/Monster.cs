using System;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [Range(0,100)]public float aggression = 0;
    [Tooltip("How fast aggression reduces normaly")]
    public float initialPacifyMult = 2f;
    [Tooltip("How fast aggression reduces when the player is quiet for long enough")]
    public float quietPacifyMult = 5f;
    float pacifyMultiplier;
    [Tooltip("How many seconds the player has to be quiet for the aggression to reduce faster")]
    public float quietTimerLimit = 5;
    float quietTimer;

    RaycastHit rayHit;
    float rayDistance = 10f;

    bool threshold1; //following footsteps
    bool threshold2; 
    bool threshold3;

    void OnEnable() {
        EventManager.SubscribeFloat("add-noise", AddAgression);
    }
    void OnDisable() {
        EventManager.UnsubscribeFloat("add-noise", AddAgression);
    }

    void Update() {
        if(!threshold1 && aggression > 0) threshold1 = true;

        if (true) StartFollowing();

        if (quietTimer > quietTimerLimit) pacifyMultiplier = quietPacifyMult;
        else pacifyMultiplier = initialPacifyMult;


        aggression -= pacifyMultiplier * Time.deltaTime;
    }

    void AddAgression(float amount) {
        aggression += amount;
        quietTimer = 0;
    }
    
    void StartFollowing() {
        GameObject player = GameObject.Find("Player");
        transform.position = player.transform.position - player.transform.forward * 15f;

        if (Physics.Raycast(transform.position, Vector3.down, out rayHit, rayDistance, LayerMask.GetMask("Ground"))) {
            Debug.Log("down cast " + rayHit.point);
            transform.position = new(transform.position.x, rayHit.point.y + 2f, transform.position.z);
        } else if (Physics.Raycast(transform.position, Vector3.up, out rayHit, rayDistance, LayerMask.GetMask("Ground"))) {
            transform.position = new(transform.position.x, rayHit.point.y + 2f, transform.position.z);
        }

    }

}
