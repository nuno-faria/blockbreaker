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


    public void LoadClassicGame() {
        GameManager.classic = true;
        SceneManager.LoadScene("GameScene");
    }


    public void LoadModernGame() {
        GameManager.classic = false;
        SceneManager.LoadScene("GameScene");
    }
}
