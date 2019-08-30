using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {

    public Text continueText;


    void Start() {
        if (PlayerPrefs.HasKey("data"))
            continueText.color = Color.white;
        else
            continueText.color = Color.gray;
    }
}
