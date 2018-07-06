using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelButton : MonoBehaviour {

    void OnMouseDown() {
        GameManager.skipLevel();    
    }
}
