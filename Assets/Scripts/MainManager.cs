using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour {

    public static MainManager mm;


    void Start() {
        mm = this;
    }


    public void LoadInstructionsScene() {
        SceneManager.LoadScene("InstructionsScene");
    }


    public void LoadPickupsScene() {
        SceneManager.LoadScene("PickupsScene");
    }


    public void LoadMenuScene() {
        SceneManager.LoadScene("MenuScene");
    }


    public void LoadGameModeSelectionScene() {
        SceneManager.LoadScene("GameModeSelectionScene");
    }


    public void LoadClassicGameScene(bool continueGame = false) {
        GameManager.classic = true;
        GameManager.large = false;
        GameManager.hardcore = false;
        GameManager.continueGame = continueGame;
        SceneManager.LoadScene("GameScene");
    }


    public void LoadModernGameScene(bool continueGame = false) {
        GameManager.classic = false;
        GameManager.large = false;
        GameManager.hardcore = false;
        GameManager.continueGame = continueGame;
        SceneManager.LoadScene("GameScene");
    }


    public void LoadModernExtraGameScene(bool continueGame = false) {
        GameManager.classic = false;
        GameManager.large = true;
        GameManager.hardcore = false;
        GameManager.continueGame = continueGame;
        SceneManager.LoadScene("GameScene");
    }

    public void LoadModernHardcoreGameScene(bool continueGame = false) {
        GameManager.classic = false;
        GameManager.large = false;
        GameManager.hardcore = true;
        GameManager.continueGame = continueGame;
        SceneManager.LoadScene("GameScene");
    }


    public void ContinueGame() {
        if (PlayerPrefs.HasKey("data")) {
            string gameMode = PlayerPrefs.GetString("mode");
            if (gameMode == "classic")
                LoadClassicGameScene(true);
            else if (gameMode == "modern")
                LoadModernGameScene(true);
            else if (gameMode == "large")
                LoadModernExtraGameScene(true);
            else
                LoadModernHardcoreGameScene(true);
        }
    }
}
