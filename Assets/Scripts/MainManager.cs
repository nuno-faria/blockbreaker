using System.Collections;
using System.Collections.Generic;
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


    public void LoadClassicGameScene() {
        GameManager.classic = true;
        GameManager.large = false;
        SceneManager.LoadScene("GameScene");
    }


    public void LoadModernGameScene() {
        GameManager.classic = false;
        GameManager.large = false;
        SceneManager.LoadScene("GameScene");
    }


    public void LoadModernExtraGameScene() {
        GameManager.classic = false;
        GameManager.large = true;
        SceneManager.LoadScene("GameScene");
    }

    public void LoadModernHardcoreGameScene() {
        GameManager.classic = false;
        GameManager.large = false;
        GameManager.hardcore = true;
        SceneManager.LoadScene("GameScene");
    }
}
