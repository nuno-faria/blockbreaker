using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseAreaManager : MonoBehaviour {

    public static Vector2 ballCenter;
    public LineRenderer lr;

    void Start() {
        lr.endWidth = 0.02f;
        lr.startWidth = 0.07f;
    }

    void OnMouseDrag() {
        if (!GameManager.ballMoving) {
            Vector3 center = new Vector3(ballCenter.x, ballCenter.y, 0);
            Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            MousePos.z = 0;
            Vector3 d = MousePos - center;
            d.Normalize();
            Vector3 p = center + (-d) * 1.5f;

            lr.positionCount = 2;
            lr.SetPosition(0, center);
            lr.SetPosition(1, p);
            lr.startColor = Color.white;
            lr.endColor = Color.white;
            lr.enabled = true;
        }
    }


    void OnMouseUp() {
        if (!GameManager.ballMoving) {
            lr.enabled = false;
            Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dir = new Vector2(MousePos.x - ballCenter.x, MousePos.y - ballCenter.y);
            dir.Normalize();
            dir = -dir;
            StartCoroutine(GameManager.throwBalls(dir));
        }
    }
}
