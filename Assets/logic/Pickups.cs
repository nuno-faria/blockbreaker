using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pickups : MonoBehaviour {

    void OnMouseDown() {
        SceneManager.LoadScene("pickupsScene");
    }
}
