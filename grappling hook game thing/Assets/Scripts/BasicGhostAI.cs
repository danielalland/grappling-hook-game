using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicGhostAI : MonoBehaviour
{
    public GameObject ghostObject;
    public GameObject ghostGoo;
    public GameObject ghostGooTrigger;
    public CapsuleCollider2D ghostCollider;
    public float speed;
    public Rigidbody2D rb;
    public float ghostBias;
    public SpriteRenderer spriteRenderer;
    public Vector3 initialForce;
    public float maxFlipSpeed;
    //public float spawnDelay;
    //public float maxSpawnDelay;

    Vector3 playerPosition;
    Vector3 ghostPosition;
    Vector3 bias;
    Vector3 force;
    float playerSpeed;
    float killSpeed;
    float timer;
    float spawnDelay;
    bool firstRun;
    bool killing;
    float flipTimer;


    // Start is called before the first frame update
    void Start()
    {
        bias.x=Random.Range(ghostBias,-ghostBias);
        bias.y=Random.Range(ghostBias,-ghostBias);
        firstRun=true;
        timer=0;
        playerPosition=PlayerSingleton.Instance.playerPosition;
        ghostPosition=ghostObject.transform.position;
        force=Vector3.Normalize(playerPosition-ghostPosition+bias)*speed*Time.deltaTime;
        //spawnDelay=Random.Range(0,maxSpawnDelay);
    }

    // Update is called once per frame
    void Update()
    {
        timer+=Time.deltaTime;
        spawnDelay=PlayerSingleton.Instance.ghostSpawnDelay;
        if(timer>spawnDelay) {
            if(firstRun==true) {
                force=initialForce;
                rb.AddForce(force);
                firstRun=false;
            }
            if(firstRun==false){
                playerPosition=PlayerSingleton.Instance.playerPosition;
                ghostPosition=ghostObject.transform.position;
                force=Vector3.Normalize(playerPosition-ghostPosition+bias)*speed*Time.deltaTime;
                //rb.velocity=Vector3.Normalize(playerPosition-ghostPosition+bias)*speed;
                rb.AddForce(force);
            }
        }
        if(force.x<0 && spriteRenderer.flipX==false) {
            if(flipTimer>=maxFlipSpeed) {
                spriteRenderer.flipX=true;
                flipTimer=0;
            }
        }else if(force.x>0 && spriteRenderer.flipX==true) {
            if(flipTimer>=maxFlipSpeed) {
                spriteRenderer.flipX=false;
                flipTimer=0;
            }
        }
        flipTimer+=Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player") {
            //playerSpeed = PlayerSingleton.Instance.playerSpeed;
            //killSpeed = PlayerSingleton.Instance.killSpeed;
            killing=PlayerSingleton.Instance.killing;
            if (killing==true) {
                ghostGooTrigger.SetActive(true);
                ghostGoo.SetActive(true);
                Destroy(ghostCollider);
                Destroy(rb);
                Destroy(spriteRenderer);
                Destroy(this);
                //spriteRenderer.flipY=true;
            }
        }
    }
}
