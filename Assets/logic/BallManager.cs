using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour {

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.name == "bottomBorder") {
            GameManager.decrementNBallsBoard();
            Destroy(this.gameObject);
        }
        else if (collision.collider.name.Contains("pickup"))
            GameManager.activatePickup(collision.collider.gameObject);
    }
}
