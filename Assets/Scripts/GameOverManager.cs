using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour {

    public Text score;

	void Start () {
        score.text = "Score: " + (GameManager.currentLevel - 1);
	}
}
