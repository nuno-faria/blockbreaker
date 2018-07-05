using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour {

    public Rigidbody2D ball;
    public LineRenderer lr;

    void Start() {
        lr.endWidth = 0.02f;
        lr.startWidth = 0.07f;
    }

    void OnMouseDrag() {
        Vector3 center = new Vector3(ball.position.x, ball.position.y, 0);
        Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        MousePos.z = 0;
        Vector3 d = MousePos - center;
        d.Normalize();
        Vector3 p = center + (-d) * 1f;

        lr.positionCount = 2;
        lr.SetPosition(0,center);
        lr.SetPosition(1,p);
        lr.startColor = Color.white;
        lr.endColor = Color.white;
        lr.enabled = true;
    }
 

    void OnMouseUp() {
        lr.enabled = false;
        Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = new Vector2(MousePos.x - ball.position.x, MousePos.y - ball.position.y);
        dir.Normalize();
        dir = -dir;
        ball.velocity = dir * 15f;
    }
}
