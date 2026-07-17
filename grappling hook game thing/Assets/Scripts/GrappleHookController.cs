using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHookController : MonoBehaviour
{
    public GameObject playerObject;
    //public GameObject playerPreviewObject;
    public LineRenderer lr;
    public Camera mainCamera;
    public Rigidbody2D rb;
    public float range;
    public float killDistance;
    public float killSpeedBonus;
    public Color grappleColor;
    public Color previewColor;
    public Color killColor;
    public Color killPreviewColor;
    public float grappleForce;
    public float grappleResetDistance;
    public float grappleResetSpeed;
    public float playerRadius;
    public float inputQueueTime;
    public float showPreviewDistance;
    
    RaycastHit2D hit;
    Vector3 playerPosition;
    Vector2 mousePosition;
    Vector2 playerPositionV2;
    int layerMask;
    //float playerRadius;
    float killSpeed;
    float playerSpeed;
    Vector2 grappleDirection;
    bool grappling;
    Vector2 playerPositionPreview;
    float initialGrappleDistance;
    public bool killing;
    Vector3 tempRotation;
    float queueTimer;
    Vector2 queuedMousePosition;
    
    // Start is called before the first frame update
    void Start()
    {
        SetVariables();
    }

    void SetVariables() {
        SetLayerMask();
        SetVariablesFromSingleton();
        //playerRadius = playerObject.transform.localScale.x/2;
    }

    void SetLayerMask() {
        layerMask = (1 << 2) | (1 << 8) | (1 << 9);
        layerMask = ~layerMask;
    }
    
    void SetVariablesFromSingleton() {
        killSpeed=PlayerSingleton.Instance.killSpeed;
        playerSpeed=PlayerSingleton.Instance.playerSpeed;
    }


    // Update is called once per frame
    void Update()
    {
        UpdateVariables();
        CheckForGrappleEnd();
        CheckForStuck();
        UpdateQueueTimer();
        if(grappling==false && queueTimer<=0) {
            grappleDirection=(mousePosition-playerPositionV2);
            grappleDirection=grappleDirection.normalized;
        }else if (grappling==false) {
            grappleDirection=(queuedMousePosition-playerPositionV2);
            grappleDirection=grappleDirection.normalized;
        }
        //hit = Physics2D.CircleCast(playerPosition, playerRadius, grappleDirection, range, layerMask);
        hit=Physics2D.Raycast(playerPosition, grappleDirection, range, layerMask);
        if(hit) {
            if((Input.GetMouseButtonDown(0) || queueTimer>0) && grappling==false) {
                queueTimer=0;
                grappling=true;
                PlayerSingleton.Instance.showPreview=false;
                initialGrappleDistance=hit.distance;
                UpdatePlayerPositonPreview();
                PlayerSingleton.Instance.nextPosition=playerPositionPreview+hit.normal*playerRadius;
                //playerPreviewObject.transform.position=playerPositionPreview;
            }else if(Input.GetMouseButtonDown(0)) {
                queueTimer=inputQueueTime;
                queuedMousePosition=mousePosition;
            }
            if(grappling==true) {
                UpdateGrappleDirection();
                if(initialGrappleDistance>=killDistance) {
                    PlayerSingleton.Instance.killing=true;
                    killing=true;
                    rb.linearVelocity=grappleDirection*killSpeedBonus;
                }else{
                    rb.linearVelocity=grappleDirection*(initialGrappleDistance+grappleForce);
                }
            }else{
                UpdatePlayerPositonPreview();
                //playerPreviewObject.transform.position=playerPositionPreview;
            }
        }
        SetLineColors();
        SetLinePositions();
        SetPlayerAngle();
        PlayerSingleton.Instance.killing=killing;
    }

    void UpdateVariables() {
        playerPosition=PlayerSingleton.Instance.playerPosition;
        PlayerSingleton.Instance.grappling=grappling;
        playerPositionV2=playerPosition;
        grappleDirection=grappleDirection.normalized;
        UpdateMousePosition();
    }
    
    void UpdateMousePosition() {
        mousePosition[0] = mainCamera.ScreenToWorldPoint(Input.mousePosition)[0];
        mousePosition[1] = mainCamera.ScreenToWorldPoint(Input.mousePosition)[1];
    }

    void UpdateGrappleDirection() {
        grappleDirection[0] = -(playerPosition.x-playerPositionPreview.x);
        grappleDirection[1] = -(playerPosition.y-playerPositionPreview.y);
        grappleDirection=grappleDirection.normalized;
    }

    void UpdatePlayerPositonPreview() {
        playerPositionPreview=grappleDirection*hit.distance+playerPositionV2;
    }

    void SetLineColors() {
        if(hit) {
            if(killing==true) {
                SetLineColor(killColor);
            }else if(grappling==true){
                SetLineColor(grappleColor);
            }else if(hit.distance>=killDistance){
                SetLineColor(killColor);
            }else {
                lr.startColor=previewColor;
                lr.endColor=Color.Lerp(previewColor,killPreviewColor,hit.distance/killDistance);
            }
        }
    }

    void SetLineColor(Color color) {
        lr.startColor=color;
        lr.endColor=color;
    }

    void SetLinePositions() {
        lr.SetPosition(0, playerPosition);
        lr.SetPosition(1, playerPositionPreview);
    }

    void CheckForStuck() {
        if(grappling==true && Mathf.Abs(rb.linearVelocity.x)<=grappleResetSpeed && Mathf.Abs(rb.linearVelocity.y)<=grappleResetSpeed) {
            grappling=false;
            killing=false;
            rb.linearVelocity=Vector2.zero;
            //Debug.Log("stuck");
        }
    }

    void SetPlayerAngle() {
        //tempRotation=(0,0,Vector2.Angle(Vector2.zero, rb.velocity)+45);
        //playerObject.transform.Rotate(new Vector3(0,0,Vector2.Angle(Vector2.zero, rb.velocity)+45), Space.World);
        playerObject.transform.rotation=Quaternion.AngleAxis(AngleFromVector2(grappleDirection)+45, Vector3.forward);
    }

    float AngleFromVector2(Vector2 vector2) {
        if (vector2.x > 0){
            return 360 - (Mathf.Atan2(vector2.x, vector2.y) * Mathf.Rad2Deg * Mathf.Sign(vector2.x));
        }
        else{
            return Mathf.Atan2(vector2.x, vector2.y) * Mathf.Rad2Deg * Mathf.Sign(vector2.x);
        }
    }

    void CheckForGrappleEnd() {
        if(Vector2.Distance(playerPositionV2,playerPositionPreview)<=grappleResetDistance && grappling==true) {
            grappling=false;
            killing=false;
            playerObject.transform.position=playerPositionPreview+(hit.normal*playerRadius);
            rb.linearVelocity=Vector2.zero;
            //Debug.Log("grappling ended normally");
        }
        if(Vector2.Distance(playerPositionV2,playerPositionPreview)<=showPreviewDistance && grappling==true) {
            PlayerSingleton.Instance.showPreview=true;
            //Debug.Log("grappling ended normally");
        }
    }

    void UpdateQueueTimer() {
        if(queueTimer>=-1) {
            queueTimer-=Time.deltaTime;
        }
    }
}
