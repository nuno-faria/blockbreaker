using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.name == "bottomBorder") {
            GameManager.gm.DecrementNBallsBoard();
            Destroy(this.gameObject);
        }
        else if (collision.collider.tag == "pickup")
            GameManager.gm.ActivatePickup(collision.collider.gameObject);
    }
}
