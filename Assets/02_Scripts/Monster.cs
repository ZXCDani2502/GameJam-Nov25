using UnityEngine;

public class Monster : MonoBehaviour {
    [Header("Aggression")]
    [Range(0, 100)] public float aggression = 0;
    [Tooltip("How fast aggression reduces normaly")]
    public float initialPacifyMult = 2f;
    [Tooltip("How fast aggression reduces when the player is quiet for long enough")]
    public float quietPacifyMult = 5f;
    float pacifyMultiplier;
    [Tooltip("How many seconds the player has to be quiet for the aggression to reduce faster")]
    public float quietTimerLimit = 5;
    float quietTimer;


    [Header("Thresholds")]
    [Tooltip("Monster starts following behind you")]
    int threshold1 = 25;
    [Tooltip("Ambience randomly mutes")]
    int threshold2;
    [Tooltip("Spawn eyes out of view")]
    int threshold3 = 98;

    bool threshold1Hit; //following footsteps
    bool threshold2Hit;
    bool threshold3Hit;

    [Header("Monster Events")]
    public float monsterBaseDistance = 15f;
    GameObject player;
    RaycastHit rayHit;
    float rayDistance = Mathf.Infinity;
    //follow
    [SerializeField] float followTimerLimit = 0.6f;
    float followTimer;
    //heavy
    int randomDirection;
    [SerializeField] float lookMonsterTimerLimit = 1.5f;
    float lookMonsterTimer;
    float distanceFromPlayerToChase = 50;

    void OnEnable() => EventManager.SubscribeFloat("add-noise", AddAgression);
    void OnDisable() => EventManager.UnsubscribeFloat("add-noise", AddAgression);
    void Start() => player = GameObject.Find("Player");

    void Update() {
        if (!threshold1Hit && aggression > threshold1) threshold1Hit = true;
        if (!threshold2Hit && aggression > threshold2) threshold2Hit = true;
        if (!threshold3Hit && aggression > threshold3) {
            threshold3Hit = true;
            TPOutOfView();
            randomDirection = Random.Range(0, 2);
        }

        if (quietTimer > quietTimerLimit) pacifyMultiplier = quietPacifyMult;
        else pacifyMultiplier = initialPacifyMult;

        if (followTimer < followTimerLimit) followTimer += Time.deltaTime;

        aggression -= pacifyMultiplier * Time.deltaTime;
    }

    void FixedUpdate() {
        if (threshold1Hit) StartFollowing();
        if (threshold3Hit) { 
        CheckLookingAtMonster();
            if (Vector3.Distance(transform.position, player.transform.position) > distanceFromPlayerToChase) {
                TPOutOfView();
            }
        }
    }

    void CheckLookingAtMonster() {
        var ct = GameObject.Find("Main Camera").transform;
        transform.LookAt(ct);
        Debug.DrawRay(ct.position, ct.forward * 30, Color.green);
        if (Physics.Raycast(ct.position, ct.forward, out _, rayDistance, LayerMask.GetMask("Monster"))) {
            if (lookMonsterTimer > lookMonsterTimerLimit) {
                EventManager.Trigger("death-state");
            }
            else 
                lookMonsterTimer += Time.deltaTime;
        }
    }

    void AddAgression(float amount) {
        aggression += amount;
        quietTimer = 0;
    }

    void StartFollowing() {
        if (threshold3Hit) return;
        transform.position = player.transform.position - player.transform.forward * monsterBaseDistance * Random.Range(0.7f, 1.3f); // a bit of variation

        if (Physics.Raycast(transform.position + new Vector3(0, 10, 0), Vector3.down, out rayHit, rayDistance, LayerMask.GetMask("Ground"))) {
            transform.position = new(transform.position.x, rayHit.point.y, transform.position.z);
        }

        if (followTimer > followTimerLimit) {
            EventManager.Trigger("sfx-follow");
            followTimer = 0;
        }//start at random intervals
    }

    void TPOutOfView() {
        var pt = player.transform;
        //spawn on either the left or the right of the player
        transform.position = randomDirection == 0 ? pt.position + pt.right * monsterBaseDistance : pt.position - pt.right * monsterBaseDistance;
        if (Physics.Raycast(transform.position + new Vector3(0, 10, 0), Vector3.down, out rayHit, rayDistance, LayerMask.GetMask("Ground")))
            transform.position = new(transform.position.x, rayHit.point.y + 2f, transform.position.z);
        transform.LookAt(pt);

        EventManager.Trigger("sfx-heavy");
    }
}
