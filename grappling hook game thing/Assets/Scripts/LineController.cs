using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    public Rigidbody2D rb;
    public float grappleResetDistance;
    public float grappleResetSpeed;
    public float grappleDistanceSpeed;
    public float inputQueueTime;
    public Color grapplingColor;
    public Color previewColor;
    public Color killColor;
    public Color killPreviewColor;
    public GameObject playerObject;
    public GameObject playerPreview;
    //public Color testGrapplingColor;
    //public Color testPreviewColor;
    //public Color testKillPreviewColor;
    public float killSpeedBonus;
    int layerMask;
    float inputQueueTracker;
    private LineRenderer lr;
    private Transform[] points;
    [SerializeField] private Camera mainCamera;
    Vector3 mousePosition;
    Vector3 queuedMousePosition;
    Vector3 playerPosition;
    public float range;
    Vector2 pointDirection;
    public float grappleForce;
    Vector2 grappleDirectionRotation;
    Vector2 grappleDirection;
    bool grappling;
    bool killing;
    Vector2 grapplePoint;
    float initialGrappleDistance;
    float killSpeed;
    float playerSpeed;
    float tempKillSpeedBonus;
    float grappleLength;
    float speedPreview;
    float playerRadius;
    Vector2 playerPositionV2;
    Vector2 playerPositionPreview;

    //Color originalGrapplingColor;
    //Color originalPreviewColor;
    //Color originalKillPreviewColor;

    //bool testingColors = false;

    private void Awake() {
        lr = GetComponent<LineRenderer>();
    }

    private void Start() {
        layerMask = (1 << 2) | (1 << 8) | (1 << 9);
        layerMask = ~layerMask;
        playerRadius = playerObject.transform.localScale.x/2;

        //originalGrapplingColor=grapplingColor;
        //originalKillPreviewColor=killPreviewColor;
        //originalPreviewColor=previewColor;
    }

    public void SetUpLine(Transform[] points) {
        lr.positionCount = points.Length;
        this.points = points;
    }

    void SetVariablesFromSingleton() {
        killSpeed=PlayerSingleton.Instance.killSpeed;
        playerSpeed=PlayerSingleton.Instance.playerSpeed;
    }

    void SetLineColor(Color color) {
        lr.startColor=color;
        lr.endColor=color;
    }

    /*Color TransitionColor(Color color1, Color color2, float t) {
        return new Color(Mathf.Lerp(color1.r,color2.r,t),Mathf.Lerp(color1.g,color2.g,t),Mathf.Lerp(color1.b,color2.b,t),Mathf.Lerp(color1.a,color2.a,t));
    }*/

    private void Update() {
        SetVariablesFromSingleton();
        playerPositionV2=playerPosition;
        /*if(Input.GetKeyDown("space")) {
            if(testingColors==false) {
                testingColors=true;
            }else{
                testingColors=false;
            }
        }
        if(testingColors==true) {
            grapplingColor=testGrapplingColor;
            previewColor=testPreviewColor;
            killPreviewColor=testKillPreviewColor;
        }else{
            grapplingColor=originalGrapplingColor;
            previewColor=originalPreviewColor;
            killPreviewColor=originalKillPreviewColor;
        }*/
        grappleLength=Vector2.Distance(playerPosition,playerPositionPreview);
        if(grappleLength <= grappleResetDistance && grappling==true) {
            grappling=false;
            killing=false;
            rb.linearVelocity=Vector2.zero;
        }
        if(grappling==true && Mathf.Abs(rb.linearVelocity[0])<=grappleResetSpeed && Mathf.Abs(rb.linearVelocity[1])<=grappleResetSpeed) {
            grappling=false;
            killing=false;
            rb.linearVelocity=Vector2.zero;
        }
        inputQueueTracker-=Time.deltaTime;
        if(grappling==false && inputQueueTracker>0) {
            mousePosition[0]=queuedMousePosition[0];
            mousePosition[1]=queuedMousePosition[1];
        }else{
            mousePosition[0] = mainCamera.ScreenToWorldPoint(Input.mousePosition)[0];
            mousePosition[1] = mainCamera.ScreenToWorldPoint(Input.mousePosition)[1];
        }
        pointDirection[0] = mousePosition[0] - playerPosition[0];
        pointDirection[1] =  mousePosition[1] - playerPosition[1];
        playerPosition = PlayerSingleton.Instance.playerPosition;
        if(grappling==false) {
            grappleDirection=pointDirection;
        }
        RaycastHit2D hit = Physics2D.Raycast(playerPosition, grappleDirection, range, layerMask);
        //RaycastHit2D hit = Physics2D.CircleCast(playerPosition, playerRadius, grappleDirection, range, layerMask);
        if (hit) {
            speedPreview = Vector2.Distance(playerPosition,hit.point)*grappleDistanceSpeed+grappleForce;
            if ((Input.GetMouseButtonDown(0) || inputQueueTracker>0) && grappling==false) {
                grappling=true;
                grapplePoint=hit.point;
                initialGrappleDistance=Vector2.Distance(playerPosition,grapplePoint)*grappleDistanceSpeed;
                inputQueueTracker=0;
            }else if(Input.GetMouseButtonDown(0)){
                queuedMousePosition[0]=mousePosition[0];
                queuedMousePosition[1]=mousePosition[1];
                inputQueueTracker=inputQueueTime;
            }
            if(grappling==true) {
                grappleDirection[0] = -(playerPosition.x-playerPositionPreview.x);
                grappleDirection[1] = -(playerPosition.y-playerPositionPreview.y);
                grappleDirection=grappleDirection.normalized;
                if(Vector2.Distance(playerPosition,hit.point)*grappleDistanceSpeed+grappleForce>=killSpeed || killing==true) {
                    rb.linearVelocity=grappleDirection*killSpeedBonus;
                    killing=true;
                    SetLineColor(killColor);
                    //tempKillSpeedBonus=killSpeedBonus;
                }else{
                    rb.linearVelocity=grappleDirection*(initialGrappleDistance+grappleForce);
                    SetLineColor(grapplingColor);
                    //tempKillSpeedBonus=0;
                }
                //rb.velocity=grappleDirection*(initialGrappleDistance+grappleForce+tempKillSpeedBonus);
                //SetLineColor(grapplingColor);
            }else if(speedPreview>=killSpeed){
                SetLineColor(killColor);
            }else{
                lr.startColor=previewColor;
                //lr.endColor=new Color(1-(speedPreview/killSpeed),1,1-(speedPreview/killSpeed),1);
                //SetLineColor(TransitionColor(previewColor,killPreviewColor,speedPreview/killSpeed));
                //SetLineColor(Color.Lerp(previewColor,killPreviewColor,speedPreview/killSpeed));
                lr.endColor=Color.Lerp(previewColor,killPreviewColor,speedPreview/killSpeed);
                //lr.startColor=new Color(0,0,0,0);
                //lr.endColor=Color.Lerp(new Color(0,0,0,0),killPreviewColor,speedPreview/killSpeed);
            }
            lr.SetPosition(0, playerPosition);
            if(grappling==true) {
                lr.SetPosition(1, hit.point);
                //playerPreview.transform.position=grappleDirection.normalized*hit.distance+playerPositionV2;
            }else{
                lr.SetPosition(1,hit.point);
                playerPositionPreview=grappleDirection.normalized*hit.distance+playerPositionV2;
            }
        } else {
            lr.SetPosition(0, playerPosition);
            lr.SetPosition(1, mousePosition);
        }
        Debug.Log(hit.distance);
    }
}