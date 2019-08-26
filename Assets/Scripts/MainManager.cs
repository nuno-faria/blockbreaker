using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour {

    public static MainManager mm;


    void Start() {
        mm = this;
    }


    public void LoadGameScene() {
        SceneManager.LoadScene("GameScene");
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


    public void ToggleSound() {
        //GameManager.gm.turnSound();
    }
}
