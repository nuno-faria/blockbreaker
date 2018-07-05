using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public GameObject blockPrefab;
    public GameObject ballPrefab;
    public Text level;
    private static List<GameObject> balls;
    private static List<GameObject> blocks;
    private static int nBalls;
        
    private static float blockSize;
    public static int currentLevel = 0;
    public static bool ballMoving = false;

    private static GameObject sBallPrefab;
    private static GameObject sBlockPrefab;

	void Start () {
        blockSize = blockPrefab.GetComponent<Renderer>().bounds.size.x;

        balls = new List<GameObject>();
        ballPrefab.transform.position = new Vector2(0, -4.3f);
        balls.Add(Instantiate(ballPrefab));

        blocks = new List<GameObject>();

        nBalls = 1;
        currentLevel = 1;
        ballMoving = false;
        level.text = "1";
        sBallPrefab = ballPrefab;
        sBlockPrefab = blockPrefab;

        generateRow(Random.Range(2, 4));
    }

    //max nBlocks = 7
    private static void generateRow(int nBlocks) {
        List<int> l = new List<int> { -3, -2, -1, 0, 1, 2, 3};
        for (int i = 0; i< nBlocks; i++) {
            int idx = Random.Range(0, l.Count);
            createBlock(l[idx]);
            l.Remove(l[idx]);
        }
    }

    private static void createBlock(int row) {
        GameObject g = Instantiate(sBlockPrefab);
        g.transform.position = new Vector2(blockSize * row, 3.6f);
        blocks.Add(g);
    }

    public static IEnumerator throwBalls(Vector2 dir) {
        ballMoving = true;
        foreach (GameObject b in balls) {
            b.GetComponent<Rigidbody2D>().velocity = dir * 15;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public static void decrementNBalls() {
        nBalls--;
        if (nBalls == 0)
            newLevel();
    }

    public static void newLevel() {
        currentLevel++;

        //blocks
        foreach (GameObject block in blocks)
            block.transform.Translate(new Vector2(0, -blockSize));
        generateRow(Random.Range(2, System.Math.Max(5, System.Math.Min(8, (currentLevel * 2) / 10))));

        //balls
        balls.Clear();
        for (int i = 0; i < currentLevel; i++)
            balls.Add(Instantiate(sBallPrefab));
        nBalls = balls.Count();
        ballMoving = false;
    }

    public static void removeBlock(GameObject block) {
        blocks.Remove(block);
    }
}
