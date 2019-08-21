using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Instructions : MonoBehaviour {

    void OnMouseDown() {
        SceneManager.LoadScene("instructionsScene");
    }
}
