using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCollisions : MonoBehaviour
{

    public Rigidbody2D rb;
    public GameObject playerObject;

    float killSpeed;
    float playerSpeed;
    bool killing;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Ghost") {
            //playerSpeed = PlayerSingleton.Instance.playerSpeed;
            //killSpeed=PlayerSingleton.Instance.killSpeed;
            killing=PlayerSingleton.Instance.killing;
            if (killing==false) {
                //Debug.Log("Death");
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
        if(collision.gameObject.tag=="GhostGoo") {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
