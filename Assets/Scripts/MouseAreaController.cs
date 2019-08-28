using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseAreaController : MonoBehaviour {

    public static Vector2 ballCenter;
    public LineRenderer line;
    public LayerMask layersToIngore;
    //Vector where the arrow is pointing
    private Vector2 p;



    void Start() {
        line.positionCount = 3;
        layersToIngore = ~layersToIngore;
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
            hit = Physics2D.CircleCast(ballCenter, 0.107f, direction, 10f, layersToIngore);

            line.SetPosition(0, center);
            line.SetPosition(1, hit.point);
            p = hit.point;

            //reflection
            hit = Physics2D.Raycast(hit.point, Vector2.Reflect(direction, hit.normal), 10f, layersToIngore);
            line.SetPosition(2, hit.point);

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
