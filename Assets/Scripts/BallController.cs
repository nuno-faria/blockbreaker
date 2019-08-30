using UnityEngine;

public class BallController : MonoBehaviour {

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.name == "bottomBorder" || (collision.collider.tag == "blockLava")) {
            GameManager.gm.DecrementNBallsBoard();
            Destroy(gameObject);
        }
        else if (collision.collider.tag == "pickup")
            GameManager.gm.ActivatePickup(collision.collider.gameObject);
    }
}
