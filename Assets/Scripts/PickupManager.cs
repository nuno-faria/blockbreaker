using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour {

    public AudioClip pickup;

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.tag == "ball")
            if (GameManager.sound)
                AudioSource.PlayClipAtPoint(pickup, new Vector2(0, 0), 0.4f);
    }
}
