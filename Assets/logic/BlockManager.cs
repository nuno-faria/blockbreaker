using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockManager : MonoBehaviour {

    public Text hpText;
    private int hp;

	void Awake () {
        hp = GameManager.currentLevel;
        hpText.text = hp.ToString();
	}

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.tag == "ball") {
            hp--;
            if (hp == 0) {
                GameManager.removeBlock(this.gameObject);
                Destroy(this.gameObject);
            }
            else
                hpText.text = hp.ToString();
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
