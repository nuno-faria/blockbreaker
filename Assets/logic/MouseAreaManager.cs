using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseAreaManager : MonoBehaviour {

    public static Vector2 ballCenter;
    public LineRenderer lr;

    //Vector where the arrow is pointing
    private Vector2 p;

    public static float pointerLength;

    void Start() {
        lr.endWidth = 0.02f;
        lr.startWidth = 0.07f;
        pointerLength = 1.5f;
    }

    void OnMouseDrag() {
        if (!GameManager.ballMoving) {
            Vector3 center = new Vector3(ballCenter.x, ballCenter.y, 0);
            Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            MousePos.z = 0;
            Vector3 d = MousePos - center;
            d.Normalize();
            p = center + (-d) * pointerLength;

            if (p.y < -4.2)
                p = new Vector2(p.x, -4.2f);

            lr.positionCount = 2;
            lr.SetPosition(0, center);
            lr.SetPosition(1, p);
            Color c;
            if (pointerLength == 3)
                c = Color.yellow;
            else c = Color.white;
            lr.startColor = c;
            lr.endColor = c;
            lr.enabled = true;
        }
    }


    void OnMouseUp() {
        if (!GameManager.ballMoving) {
            lr.enabled = false;
            Vector2 dir = new Vector2(p.x - ballCenter.x, p.y - ballCenter.y);
            dir.Normalize();
            StartCoroutine(GameManager.throwBalls(dir));
        }
    }
}
