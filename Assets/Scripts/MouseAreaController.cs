using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseAreaController : MonoBehaviour {

    public static Vector2 ballCenter;
    public LineRenderer line;
    public LayerMask layersToIngore;
    //Vector where the line is pointing
    private Vector2 p;
    private float ballRadius;



    void Start() {
        if (!GameManager.hardcore)
            line.positionCount = 3;
        else
            line.positionCount = 2;
        layersToIngore = ~layersToIngore;
        ballRadius = !GameManager.large ? 0.107f : 0.083f;
    }


    void OnMouseDrag() {
        if (!GameManager.gm.ballMoving) {
            Vector3 center = new Vector3(ballCenter.x, ballCenter.y, 0);
            Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            MousePos.z = 0;
            Vector3 direction = MousePos - center;
            direction.Normalize();
            direction = -direction;

            if (direction.y < 0.2f)
                direction.y = 0.2f;

            RaycastHit2D hit;
            hit = Physics2D.CircleCast(ballCenter, ballRadius, direction, 10f, layersToIngore);

            line.SetPosition(0, center);
            line.SetPosition(1, hit.point);
            p = hit.point;

            //reflection
            if (!GameManager.hardcore) {
                hit = Physics2D.Raycast(hit.point, Vector2.Reflect(direction, hit.normal), 10f, layersToIngore);
                line.SetPosition(2, hit.point);
            }

            line.enabled = true;
        }
    }


    void OnMouseUp() {
        if (!GameManager.gm.ballMoving) {
            line.enabled = false;
            Vector2 dir = new Vector2(p.x - ballCenter.x, p.y - ballCenter.y);
            dir.Normalize();
            StartCoroutine(GameManager.gm.ThrowBalls(dir));
        }
    }
}
