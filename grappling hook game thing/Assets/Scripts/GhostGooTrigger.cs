using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostGooTrigger : MonoBehaviour
{
    public CircleCollider2D ghostGooCollider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player") {
            ghostGooCollider.enabled=true;
        }
    }
}
