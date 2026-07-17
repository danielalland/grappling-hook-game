using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayBarrierPlayerDetector : MonoBehaviour
{
    public bool playerColliding;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player") {
            playerColliding=true;
        }
    }
    void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player") {
            playerColliding=false;
        }
    }
}
