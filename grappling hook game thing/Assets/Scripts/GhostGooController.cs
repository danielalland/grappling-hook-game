using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostGooController : MonoBehaviour
{

    public float disableTime;
    public GameObject ghostObject;
    public GameObject ghostGooObject;
    public CircleCollider2D ghostGooCollider;
    public float shrinkSpeed;
    public float minScale;

    float timer;
    //float invincibleTime;
    float shrinkScale;
    Vector3 scaleTemp;

    // Start is called before the first frame update
    void Start()
    {
        ghostGooCollider.enabled=false;
        //invincibleTime=PlayerSingleton.Instance.ghostGooInvincibleTime;
    }

    // Update is called once per frame
    void Update()
    {
        timer+=Time.deltaTime;
        /*if(timer>=disableTime) {
            Destroy(ghostObject);
        }*/
        shrinkScale=(disableTime-timer)/disableTime;
        scaleTemp=ghostGooObject.transform.localScale;
        if(scaleTemp.x>minScale && scaleTemp.y>minScale){
            scaleTemp.x-=shrinkSpeed*Time.deltaTime;
            scaleTemp.y-=shrinkSpeed*Time.deltaTime;
        }else{
            Destroy(ghostObject);
        }
        ghostGooObject.transform.localScale=scaleTemp;
    }
}
