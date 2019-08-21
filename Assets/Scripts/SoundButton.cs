using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundButton : MonoBehaviour {

    void OnMouseDown() {
        GameManager.turnSound();    
    }
}
