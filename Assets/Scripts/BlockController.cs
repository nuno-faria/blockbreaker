using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockController : MonoBehaviour {

    public Text hpText;
    public AudioClip hitClip;
    public int hp;
    public string type = "normal";

	void Awake () {
        float rand = Random.Range(0f, 1f);
        if (rand <= 0.95)
            hp = GameManager.gm.currentLevel;
        else hp = GameManager.gm.currentLevel * 2;
        hpText.text = hp.ToString();

        //color
        if (type == "normal")
            GetComponent<SpriteRenderer>().color = Color.HSVToRGB(System.Math.Min(hp / 200f, 0.92f), 1f, 1f);
	}


    private void OnCollisionEnter2D(Collision2D collision) {
        ProcessHit(collision.collider);
    }


    //water block
    private void OnTriggerEnter2D(Collider2D collider) {
        ProcessHit(collider);
    }

    
    private void ProcessHit(Collider2D collider) {
        if (collider.tag == "ball") {
            hp--;
            if (GameManager.gm.sound)
                AudioSource.PlayClipAtPoint(hitClip, new Vector2(0, 0), 0.4f);

            if (hp == 0) {
                GameManager.gm.RemoveBlock(gameObject);
                Destroy(gameObject);
            }
            else {
                hpText.text = hp.ToString();
                if (type == "normal")
                    GetComponent<SpriteRenderer>().color = Color.HSVToRGB(System.Math.Min(hp / 200f, 0.92f), 1f, 1f);
            }
        }
    }


    public void halveHP() {
        if (hp > 1) {
            hp /= 2;
            hpText.text = hp.ToString();
            if (type == "normal")
                GetComponent<SpriteRenderer>().color = Color.HSVToRGB(System.Math.Min(hp / 200f, 0.92f), 1f, 1f);
        }
    }
}
