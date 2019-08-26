using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonController : MonoBehaviour {

    public UnityEvent onMouseDown;


    void OnMouseDown() {
        onMouseDown.Invoke();    
    }
}
