using UnityEngine;
using System.Collections.Generic;

public class Preview : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public int checkCount;
    public int searchCount;
    public float range;
    public float checkSmear;
    public float searchSmear;
    public GameObject grappleHook;
    public GameObject goodPreview;
    public GameObject badPreview;
    List<RaycastHit2D> samples;
    Vector3 playerPosition;
    int layerMask;
    Vector3 previousSlope;
    Vector3 previousPoint;
    public List<LineRenderer> lr;
    List<GameObject> lrobjs = new List<GameObject>();
    int points;
    List<Vector3> directions;
    float killDistance;
    bool isKillDistance;
    bool wasKillDistance;


    void SetLayerMask() {
        layerMask = (1 << 2) | (1 << 8) | (1 << 9);
        layerMask = ~layerMask;
    }

    async Awaitable CreatePreview()
    {
        InitializeLists();
        playerPosition = PlayerSingleton.Instance.nextPosition;
        await Check();
        await Search();
        await RenderPreview();
    }

    void DestroyLineRenderers() {
        foreach (GameObject i in lrobjs) {
            DestroyImmediate(i);
        }
        lrobjs.Clear();
    }

    async Awaitable Check()
    {
        int totalSteps = Mathf.Max(1, checkCount + 2);
        float angleDivisor = Mathf.Max(1, checkCount);
        double startTime = Time.realtimeSinceStartupAsDouble;
        for(int i = 0; i < totalSteps; i++) {
            float y=Mathf.Sin((float)i*(2*Mathf.PI)/angleDivisor);
            float x=Mathf.Cos((float)i*(2*Mathf.PI)/angleDivisor);
            directions.Add(new Vector3(x,y,0));
            RaycastHit2D sample = Physics2D.Raycast(playerPosition, new Vector3(x,y,0), range, layerMask);
            samples.Add(sample);
            await Smear(i + 1, totalSteps, checkSmear, startTime);
        }
    }

    async Awaitable Search()
    {
        if (samples.Count == 0 || directions.Count == 0) {
            return;
        }

        int estimatedTotalSteps = Mathf.Max(1, searchCount * samples.Count);
        int completedSteps = 0;
        double startTime = Time.realtimeSinceStartupAsDouble;

        for(int j=0; j<searchCount; j++) {
            estimatedTotalSteps = Mathf.Max(estimatedTotalSteps, searchCount * samples.Count);
            Vector2 prevNormal = samples[0].normal;
            bool prevKilling;
            if(samples[0].distance>=killDistance){
                prevKilling=true;
            }else{
                prevKilling=false;
            }
            for(int i=0; i<samples.Count&&i<directions.Count; i++) {
                estimatedTotalSteps = Mathf.Max(estimatedTotalSteps, searchCount * samples.Count);
                bool killing;
                if(samples[i].distance>=killDistance){
                    killing=true;
                }else{
                    killing=false;
                }
                if(i > 0 && (samples[i].normal!=prevNormal||killing!=prevKilling)) {
                    prevNormal=samples[i].normal;
                    prevKilling=killing;
                    directions.Insert(i, (directions[i]+directions[i-1])/2);
                    samples.Insert(i,Physics2D.Raycast(playerPosition, directions[i], range, layerMask));
                }
                completedSteps++;
                await Smear(completedSteps, estimatedTotalSteps, searchSmear, startTime);
            }
        }
    }

    async Awaitable Smear(int completedSteps, int totalSteps, float smearSeconds, double startTime)
    {
        if (smearSeconds <= 0f || totalSteps <= 0) {
            return;
        }

        float progress = Mathf.Clamp01((float)completedSteps / totalSteps);
        double desiredElapsed = progress * smearSeconds;
        while (Time.realtimeSinceStartupAsDouble - startTime < desiredElapsed) {
            await Awaitable.NextFrameAsync();
        }
    }

    async Awaitable RenderPreview()
    {
        while(!PlayerSingleton.Instance.showPreview) {
            await Awaitable.NextFrameAsync();
        }
        Vector3 previousNormal = samples[0].normal;
        int sameSlope=0;
        int lines = 0;
        DestroyLineRenderers();
        if(PlayerSingleton.Instance.showPreview){
            for(int i = 0; i<samples.Count; i++) {
                Vector3 x = samples[i].point;
                Vector3 point = new Vector3(x.x, x.y, 1);
                Vector3 normal = samples[i].normal;
                if(i>0) {
                    if (previousNormal==normal) {
                        if(samples[i].distance>=killDistance){
                            isKillDistance=true;
                        }else{
                            isKillDistance=false;
                        }
                        if (sameSlope == 0||isKillDistance!=wasKillDistance) {
                            RenderNewLine(lines, point);
                            lines++;
                        }else{
                            lr[lines-1].SetPosition(1, point);
                        }
                        sameSlope++;
                    }else{
                        sameSlope=0;
                    }
                    previousNormal=normal;
                }
                previousPoint = point;
                previousNormal=normal;
                wasKillDistance=isKillDistance;
            }
        }else{
            await Awaitable.NextFrameAsync();
        }
    }

    void RenderNewLine(int lines, Vector3 point)
    {
        GameObject lineObj;
        if(isKillDistance){
            lineObj = Instantiate(goodPreview, Vector3.zero, Quaternion.identity);
        }else{
            lineObj = Instantiate(badPreview, Vector3.zero, Quaternion.identity);
        }
        lrobjs.Add(lineObj);
        lineObj.transform.SetParent(this.transform);
        lineObj.name=lines.ToString();
        LineRenderer lrl = lineObj.GetComponent<LineRenderer>();
        lr.Add(lrl);
        lr[lines].positionCount=2;
        lr[lines].SetPosition(0, previousPoint);
        lr[lines].SetPosition(1, point);
    }

    void InitializeLists()
    {
        samples = new List<RaycastHit2D>();
        lr=new List<LineRenderer>();
        directions = new List<Vector3>();
    }

    public async void Start() {
        SetLayerMask();
        killDistance = grappleHook.GetComponent<GrappleHookController>().killDistance;
        while(true) {
            if (PlayerSingleton.Instance.grappling) {
                await CreatePreview();
            }
            await Awaitable.NextFrameAsync();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
