using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayBarrierController : MonoBehaviour
{

    public GameObject barrierObject;
    public SpriteRenderer spriteRenderer;
    public BoxCollider2D barrierCollider;
    public bool rightGood;
    public OneWayBarrierPlayerDetector playerDetector;

    Vector3 playerPosition;
    Vector3 barrierPosition;
    float barrierSlope;
    float yIntercept;

    // Start is called before the first frame update
    void Start()
    {
        barrierSlope=Mathf.Tan(barrierObject.transform.eulerAngles.z*Mathf.Deg2Rad);
        barrierPosition=barrierObject.transform.position;
        yIntercept=barrierPosition.y-barrierSlope*barrierPosition.x;
        /*if(rightGood==false) {
            spriteRenderer.flipY=true;
        }else{
            spriteRenderer.flipY=false;
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        playerPosition=PlayerSingleton.Instance.playerPosition;
        if(rightGood==true) {
            if(playerPosition.y>=barrierSlope*playerPosition.x+yIntercept) {
                //spriteRenderer.color=Color.green;
                barrierCollider.enabled=false;
            }else{
                //spriteRenderer.color=Color.red;
                if(playerDetector.playerColliding==false) {
                    barrierCollider.enabled=true;
                }
            }
        }else{
            if(playerPosition.y<=barrierSlope*playerPosition.x+yIntercept) {
                //spriteRenderer.color=Color.green;
                barrierCollider.enabled=false;
            }else{
                //spriteRenderer.color=Color.red;
                if(playerDetector.playerColliding==false) {
                    barrierCollider.enabled=true;
                }
            }
        }
    }
}
