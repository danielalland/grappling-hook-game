using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public struct enemyQueue {
        public subWave[] subWaves;
        public float[] powerUpSpawnChances;
    }

    [System.Serializable]
    public struct subWave {
        public float[] enemies;
        public float[] powerUps;
    }

    public GameObject RedGhost;
    public GameObject YellowGhost;
    public GameObject indicatorObject;
    public GameObject ghostGooClearObject;
    public int maxGhostGooClears;
    public float loopDelay;
    public Vector2 spawnAreaPoint1;
    public Vector2 spawnAreaPoint2;
    public float indicatorDistance;
    public float cornerAvoidance;
    public float initialForce;

    public enemyQueue[] waves = new enemyQueue[1];

    int wave=0;
    bool waveActive;

    float spawnWidth;
    float spawnHeight;
    float halfSpawnPerimeter;
    float onHorizontalProbability;
    Vector3 tempGhostPosition;
    bool onTop;
    bool onBottom;
    bool onLeft;
    bool onRight;
    Vector2 playerPosition;
    int test;

    [SerializeField]
    List<GameObject> killableEnemies = new List<GameObject>();
    List<GameObject> ghostGooClears = new List<GameObject>();

    int unkillableEnemies;

    IEnumerator SpawnEnemies() {
        while(wave<waves.Length) {
            //Debug.Log(wave);
            RemoveDeadEnemies();
            if(killableEnemies.Count<=0) {
                waveActive=false;
                wave++;
            }else{
                waveActive=true;
            }
            if (waveActive==false) {
                for(int i = 0; i< waves[wave].subWaves.Length; i++) {
                    for(int j=0; j<waves[wave].subWaves[i].enemies.Length; j++) {
                        switch(j) {
                            case 0: // Delay
                                //Debug.Log("StartingDelay");
                                yield return new WaitForSeconds(waves[wave].subWaves[i].enemies[j]);
                                //Debug.Log("Delay finished after "+waves[wave].subWaves[i].enemies[j].ToString());
                                break;
                            case 1: // Red Ghost
                                SpawnGhost(waves[wave].subWaves[i].enemies[j],RedGhost,indicatorObject);
                                break;
                            case 2: // Yellow Ghost
                                SpawnGhost(waves[wave].subWaves[i].enemies[j],YellowGhost,indicatorObject);
                                break;
                            case 3:
                                
                            default:
                                //Debug.Log("Other");
                                break;
                        }
                    }
                }
                //Debug.Log(wave);
                waveActive=true;
                //wave++;
            }
            yield return new WaitForSeconds(loopDelay);
        }
        yield break;
    }

    IEnumerator SpawnPowerUps() {
        while(wave<waves.Length) {
            for(int i=0; i<waves[wave].powerUpSpawnChances.Length; i++) {
                switch(i) {
                    case 0: // Ghost Goo clear
                        SpawnPowerUp(waves[wave].powerUpSpawnChances[i], ghostGooClearObject, ghostGooClears, maxGhostGooClears);
                        break;
                    default:
                        break;
                }
            }
            yield return new WaitForSeconds(loopDelay);
        }
        yield break;
    }

    Vector3 powerUpPosition() {
        test++;
        return new Vector3(test*1,0,0);
    }

    void SpawnPowerUp(float powerUpSpawnChance, GameObject powerUpObject, List<GameObject> powerUpList, int maxPowerUps) {
        RemoveUsedPowerUps(powerUpList);
        if(powerUpList.Count < maxPowerUps) {
            if(Random.Range(0f,1f)<=powerUpSpawnChance*loopDelay) {
                GameObject powerUp = Instantiate(powerUpObject, powerUpPosition(), Quaternion.identity);
                powerUpList.Add(powerUp);
            }
        }
    }

    void RemoveUsedPowerUps (List<GameObject> powerUpList) {
        for(int i=0; i<powerUpList.Count; i++) {
            if(powerUpList[i]==null) {
                powerUpList.RemoveAt(i);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GetSpawnProbabilities();
        StartCoroutine(SpawnEnemies());
        StartCoroutine(SpawnPowerUps());
    }

    void GetSpawnProbabilities() {
        spawnWidth=spawnAreaPoint2.x-spawnAreaPoint1.x-cornerAvoidance;
        spawnHeight=spawnAreaPoint2.y-spawnAreaPoint1.y-cornerAvoidance;
        halfSpawnPerimeter=spawnWidth+spawnHeight;
        onHorizontalProbability=spawnWidth/halfSpawnPerimeter;
    }

    void SpawnGhost(float ghosts,GameObject ghost,GameObject indicator) {
        for(int i=0; i<ghosts; i++) {
            tempGhostPosition=ghostPosition();
            GameObject enemy=Instantiate(ghost,tempGhostPosition, Quaternion.identity);
            GameObject indicatorInstance=Instantiate(indicatorObject,indicatorPosition(tempGhostPosition),IndicatorRotation());
            killableEnemies.Add(enemy);
            var ghostAI=enemy.GetComponent<BasicGhostAI>();
            ghostAI.initialForce=GetInitialForce(tempGhostPosition);
            ResetOnWallStatus();
        }
    }

    Vector3 ghostPosition() {
        if(Random.Range(0f,1f)<=onHorizontalProbability) {
            if(Random.Range(0,1f)<=0.5f) {
                onRight=true;
                return new Vector3(spawnAreaPoint1.x,Random.Range(spawnAreaPoint1.y-cornerAvoidance,spawnAreaPoint2.y+cornerAvoidance),0);
            }else{
                onLeft=true;
                return new Vector3(spawnAreaPoint2.x,Random.Range(spawnAreaPoint1.y-cornerAvoidance,spawnAreaPoint2.y+cornerAvoidance),0);
            }
        }else{
            if(Random.Range(0f,1f)<=0.5f) {
                onTop=true;
                return new Vector3(Random.Range(spawnAreaPoint1.x-cornerAvoidance,spawnAreaPoint2.x+cornerAvoidance),spawnAreaPoint1.y,0);
            }else{
                onBottom=true;
                return new Vector3(Random.Range(spawnAreaPoint1.x-cornerAvoidance,spawnAreaPoint2.x+cornerAvoidance),spawnAreaPoint2.y,0);
            }
        }
    }

    Vector3 indicatorPosition(Vector3 enemyPos) {
        if(enemyPos.x==spawnAreaPoint1.x) {
            return new Vector3(enemyPos.x-indicatorDistance,enemyPos.y,0);
        }else if(enemyPos.x==spawnAreaPoint2.x) {
            return new Vector3(enemyPos.x+indicatorDistance,enemyPos.y,0);
        }else if(enemyPos.y==spawnAreaPoint1.y) {
            return new Vector3(enemyPos.x,enemyPos.y-indicatorDistance,0);
        }else if (enemyPos.y==spawnAreaPoint2.y) {
            return new Vector3(enemyPos.x,enemyPos.y+indicatorDistance,0);
        }else{
            return Vector3.zero;
        }
        /*switch(enemyPos.x) {
            case spawnAreaPoint1.x:
                return new Vector3(enemyPos.x-indicatorDistance,enemyPos.y,0);
                break;
            case spawnAreaPoint2.x:
                return new Vector3(enemyPos.x+indicatorDistance,enemyPos.y,0);
                break;
            default:
                switch(enemyPos.y) {
                    case spawnAreaPoint1.y:
                        return new Vector3(enemyPos.x,enemyPos.y-indicatorDistance,0);
                        break;
                    case spawnAreaPoint2.y:
                        return new Vector3(enemyPos.x,enemyPos.y+indicatorDistance,0);
                        break;
                }
                return Vector3.zero;
                break;
        }*/
    }

    Vector3 GetInitialForce(Vector3 enemyPos) {
        if(enemyPos.x==spawnAreaPoint1.x) {
            return new Vector3(-initialForce,0,0);
        }else if(enemyPos.x==spawnAreaPoint2.x) {
            return new Vector3(initialForce,0,0);
        }else if(enemyPos.y==spawnAreaPoint1.y) {
            return new Vector3(0,-initialForce,0);
        }else if (enemyPos.y==spawnAreaPoint2.y) {
            return new Vector3(0,initialForce,0);
        }else{
            return Vector3.zero;
        }
    }

    void ResetOnWallStatus() {
        onTop=false;
        onBottom=false;
        onLeft=false;
        onRight=false;
    }

    Quaternion IndicatorRotation() {
        if(onTop==true) {
            return Quaternion.identity;
        }
        else if(onBottom==true) {
            return Quaternion.AngleAxis(180, Vector3.forward);
        }
        else if(onLeft==true) {
            return Quaternion.AngleAxis(90, Vector3.forward);
        }else if(onRight==true) {
            return Quaternion.AngleAxis(270, Vector3.forward);
        }else{
            return Quaternion.identity;
        }
    }

    void RemoveDeadEnemies() {
        for(int i=0; i<killableEnemies.Count; i++) {
            if(killableEnemies[i]==null) {
                killableEnemies.RemoveAt(i);
            }else if (killableEnemies[i].GetComponent<CapsuleCollider2D>()==null) {
                killableEnemies.RemoveAt(i);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*RemoveDeadEnemies();
        if(killableEnemies.Count<=0) {
            waveActive=false;
        }else{
            waveActive=true;
        }
        if (waveActive==false) {
            for(int i = 0; i< waves[wave].subWaves.Length; i++) {
                for(int j=0; j<waves[wave].subWaves[i].enemies.Length; j++) {
                    switch(j) {
                        case 0: // Delay
                            
                            break;
                        case 1: // Red Ghost
                            SpawnGhost(waves[wave].subWaves[i].enemies[j],RedGhost);
                            break;
                        case 2: // Yellow Ghost
                            SpawnGhost(waves[wave].subWaves[i].enemies[j],YellowGhost);
                            break;
                        default:
                            //Debug.Log("Other");
                            break;
                    }
                }
            }
            Debug.Log(wave);
            waveActive=true;
            wave++;
        }*/
    }
}
