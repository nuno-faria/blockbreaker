using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverButton : MonoBehaviour {

    void OnMouseDown() {
        SceneManager.LoadScene("menuScene");    
    }
}
