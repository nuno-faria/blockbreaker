using UnityEngine;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour {

    public Text score;

	void Start () {
        score.text = "Score: " + (GameManager.gm.currentLevel - 1);
	}
}
