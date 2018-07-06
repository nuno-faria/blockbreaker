using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public GameObject blockPrefab;
    public GameObject ballPrefab;
    public GameObject mouseArea;
    public Text level;
    public GameObject soundObject;
    public Sprite soundOn;
    public Sprite soundOff;

    private static List<GameObject> balls;
    private static List<GameObject> blocks;
    private static Dictionary<GameObject, int> blockDepths;
    private static int nBalls;
        
    private static float blockSize;
    public static int currentLevel = 0;
    public static bool ballMoving = false;
    public static bool sound;

    private static GameObject sBallPrefab;
    private static GameObject sBlockPrefab;
    private static GameObject sMouseArea;
    private static Text sLevel;
    private static GameObject sSoundObject;
    private static Sprite sSoundOn;
    private static Sprite sSoundOff;

    void Start () {
        blockSize = blockPrefab.GetComponent<Renderer>().bounds.size.x;

        balls = new List<GameObject>();
        ballPrefab.transform.position = new Vector2(0, -4.3f);
        balls.Add(Instantiate(ballPrefab));

        blocks = new List<GameObject>();
        blockDepths = new Dictionary<GameObject, int>();

        nBalls = 1;
        currentLevel = 1;
        ballMoving = false;
        level.text = "1";
        sound = true;

        sBallPrefab = ballPrefab;
        sBlockPrefab = blockPrefab;
        sMouseArea = mouseArea;
        sLevel = level;
        sSoundObject = soundObject;
        sSoundOn = soundOn;
        sSoundOff = soundOff;

        MouseAreaManager.ballCenter = ballPrefab.transform.position;

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
        blockDepths.Add(g, 1);
    }

    public static IEnumerator throwBalls(Vector2 dir) {
        ballMoving = true;
        sMouseArea.GetComponent<SpriteRenderer>().enabled = false;
        foreach (GameObject b in balls.ToList()) {
            b.GetComponent<Rigidbody2D>().velocity = dir * 18;
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
        sLevel.text = currentLevel.ToString();


        //check if game over
        foreach (GameObject block in blockDepths.Keys.ToList()) {
            if (blockDepths[block] == 10)
                SceneManager.LoadScene("gameOverScene");
            else
                blockDepths[block]++;
        }


        //blocks
        foreach (GameObject block in blocks)
            block.transform.Translate(new Vector2(0, -blockSize));

        if (currentLevel < 20)
            generateRow(Random.Range(3, 6));
        else {
            int x = Random.Range(0, 100);
            if (x < 30) generateRow(4);
            else if (x >= 30 && x < 75) generateRow(5);
            else if (x >= 75 && x < 90) generateRow(6);
            else if (x >= 90) generateRow(7);
        }


        //balls and mouse area
        float randomX = Random.Range(-2f, 2f);
        Vector2 center = new Vector2(randomX, sBallPrefab.transform.position.y);
        sBallPrefab.transform.position = center;
        sMouseArea.transform.position = new Vector3(center.x, center.y, -0.01f);
        sMouseArea.GetComponent<SpriteRenderer>().enabled = true;
        MouseAreaManager.ballCenter = center;

        balls.Clear();
        for (int i = 0; i < currentLevel; i++)
            balls.Add(Instantiate(sBallPrefab));
        nBalls = balls.Count();
        ballMoving = false;
    }

    public static void removeBlock(GameObject block) {
        blocks.Remove(block);
        blockDepths.Remove(block);
    }

    public static void skipLevel() {
        foreach (GameObject ball in balls)
            Destroy(ball);
        nBalls = 0;
        newLevel();
    }

    public static void turnSound() {
        sound = !sound;

        if (sound)
            sSoundObject.GetComponent<SpriteRenderer>().sprite = sSoundOn;

        else sSoundObject.GetComponent<SpriteRenderer>().sprite = sSoundOff;
    }
}
