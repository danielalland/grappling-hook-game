using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSingleton : MonoBehaviour
{

    public static PlayerSingleton Instance;

    public GameObject playerObject;
    public Rigidbody2D rb;
    public float playerSpeed;
    public float killSpeed;
    public float ghostSpawnDelay;
    public float ghostGooInvincibleTime;
    public bool killing;
    public bool showPreview;
    public bool grappling;
    public float grapplingDistance;
    public Vector3 nextPosition;
    //public GrappleHookController grappleHook;

    public Vector3 playerPosition;

    void Awake() {
        if (Instance != null && Instance != this) { 
            Destroy(this); 
        }
        else { 
            Instance = this; 
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        nextPosition= new Vector3(playerObject.transform.position.x, playerObject.transform.position.y, 0);
    }

    // Update is called once per frame
    void Update()
    {
        playerPosition = rb.transform.position;
        playerSpeed = Mathf.Sqrt(rb.linearVelocity.x*rb.linearVelocity.x+rb.linearVelocity.y*rb.linearVelocity.y);
        //killing=grappleHook.killing;
    }
}
