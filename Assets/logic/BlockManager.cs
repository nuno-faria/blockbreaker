using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockManager : MonoBehaviour {

    public Text hpText;
    public AudioClip hitClip;
    private int hp;

	void Awake () {
        float rand = Random.Range(0f, 1f);
        if (rand <= 0.95)
            hp = GameManager.currentLevel;
        else hp = GameManager.currentLevel * 2;
        hpText.text = hp.ToString();

        this.GetComponent<SpriteRenderer>().color = Color.HSVToRGB(System.Math.Min(hp/100f,0.92f), 1f, 1f);
	}

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.tag == "ball") {
            if (GameManager.sound)
                AudioSource.PlayClipAtPoint(hitClip, new Vector2(0, 0), 0.4f);
            hp--;
            if (hp == 0) {
                GameManager.removeBlock(this.gameObject);
                Destroy(this.gameObject);
            }
            else {
                hpText.text = hp.ToString();
                this.GetComponent<SpriteRenderer>().color = Color.HSVToRGB(System.Math.Min(hp / 200f, 0.92f), 1f, 1f);
            }
        }
    }
}
