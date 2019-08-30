using UnityEngine;
using UnityEngine.Events;

public class ButtonController : MonoBehaviour {

    public UnityEvent onMouseDown;
    public UnityEvent onMouseEnter;


    void OnMouseDown() {
        onMouseDown?.Invoke();    
    }

    void OnMouseEnter() {
        onMouseEnter?.Invoke();
    }
}
