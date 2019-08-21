using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    //unity objects
    public GameObject blockPrefab;
    public GameObject ballPrefab;
    public GameObject mouseArea;
    public Text level;
    public GameObject soundObject;
    public Sprite soundOn;
    public Sprite soundOff;
    public GameObject pickupPrefab;
    public List<Sprite> pickupsSprites;

    //game vars
    private static List<GameObject> balls;
    private static List<GameObject> blocks;
    private static Dictionary<GameObject, int> objectsDepths; //blocks and pickups
    private static List<GameObject> pickups;
    private static int nBallsBoard;
    private static float blockSize;
    public static int currentLevel = 0;
    public static bool ballMoving = false;
    public static bool sound;
    public static int nBalls;
    private static bool doubleBalls;
    private static bool noSpawn;


    //static unity objects
    private static GameObject sBallPrefab;
    private static GameObject sBlockPrefab;
    private static GameObject sMouseArea;
    private static Text sLevel;
    private static GameObject sSoundObject;
    private static Sprite sSoundOn;
    private static Sprite sSoundOff;
    private static GameObject sPickupPrefab;
    private static List<Sprite> sPickupsSprites;


    void Start () {
        blockSize = blockPrefab.GetComponent<Renderer>().bounds.size.x;

        balls = new List<GameObject>();
        ballPrefab.transform.position = new Vector2(0, -4.3f);
        balls.Add(Instantiate(ballPrefab));

        blocks = new List<GameObject>();
        objectsDepths = new Dictionary<GameObject, int>();

        pickups = new List<GameObject>();

        nBalls = 1;
        nBallsBoard = 1;
        currentLevel = 1;
        ballMoving = false;
        level.text = "1";
        sound = true;
        doubleBalls = false;

        sBallPrefab = ballPrefab;
        sBlockPrefab = blockPrefab;
        sMouseArea = mouseArea;
        sLevel = level;
        sSoundObject = soundObject;
        sSoundOn = soundOn;
        sSoundOff = soundOff;
        sPickupPrefab = pickupPrefab;
        sPickupsSprites = pickupsSprites;

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

        //pickup
        if (l.Count > 0) {
            int x = Random.Range(0, 100);
            if (x < 15)
                createPickup(l[Random.Range(0,l.Count)]);
        }
    }

    private static void createBlock(int col) {
        GameObject g = Instantiate(sBlockPrefab);
        g.transform.position = new Vector2(blockSize * col, 3.6f);
        blocks.Add(g);
        objectsDepths.Add(g, 1);
    }

    private static void createPickup(int col) {
        string sprite = "";
        int x = Random.Range(0, 100);

        if (x < 20) sprite = "plus-one";
        else if (x >= 20 && x < 40) sprite = "plus-two";
        else if (x >= 40 && x < 50) sprite = "plus-three";
        else if (x >= 50 && x < 68) sprite = "2x";
        else if (x >= 68 && x < 80) sprite = "no-spawn";
        else if (x >= 80 && x < 86) sprite = "halve-blocks";
        else if (x >= 86 && x < 88) sprite = "clear-map";
        else if (x >= 88) sprite = "bigger-pointer";

        if (sprite == "bigger-pointer" && (MouseAreaManager.pointerLength == 3 || (pickups.Count > 0 && pickups.Where(p => p.GetComponent<SpriteRenderer>().sprite.name == "bigger-pointer_pickup").Count() == 1)))
            sprite = "plus-one";

        sprite += "_pickup";

        GameObject g = Instantiate(sPickupPrefab);
        g.GetComponent<SpriteRenderer>().sprite = sPickupsSprites.Where(s => s.name == sprite).First();
        g.transform.position = new Vector2(blockSize * col, 3.6f);
        pickups.Add(g);
        objectsDepths.Add(g, 1);
    }

    public static IEnumerator throwBalls(Vector2 dir) {
        ballMoving = true;
        sMouseArea.GetComponent<SpriteRenderer>().enabled = false;
        foreach (GameObject b in balls.ToList()) {
            b.GetComponent<Rigidbody2D>().velocity = dir * 18;
            yield return new WaitForSeconds(0.07f);
        }
    }

    public static void decrementNBallsBoard() {
        nBallsBoard--;
        if (nBallsBoard == 0)
            newLevel();
    }

    public static void newLevel() {
        currentLevel++;
        sLevel.text = currentLevel.ToString();


        //check if game over
        foreach (GameObject block in objectsDepths.Keys.Where(x => blocks.Contains(x)).ToList()) {
            if (objectsDepths[block] == 10)
                SceneManager.LoadScene("gameOverScene");
            else
                objectsDepths[block]++;
        }


        //blocks and pickups
        foreach (GameObject block in blocks)
            block.transform.Translate(new Vector2(0, -blockSize));

        //pickups
        foreach (GameObject pickup in pickups.ToList()) {
            if (objectsDepths[pickup] == 6) {
                objectsDepths.Remove(pickup);
                pickups.Remove(pickup);
                Destroy(pickup);
            }
            else
                objectsDepths[pickup]++;
        }

        foreach (GameObject pickup in pickups)
            pickup.transform.Translate(new Vector2(0, -blockSize));

        if (!noSpawn) {
            if (currentLevel < 20 && !noSpawn)
                generateRow(Random.Range(3, 6));
            else {
                int x = Random.Range(0, 100);
                if (x < 5) generateRow(3);
                else if (x >= 5 && x < 35) generateRow(4);
                else if (x >= 35 && x < 80) generateRow(5);
                else if (x >= 80 && x < 95) generateRow(6);
                else if (x >= 95) generateRow(7);
            }
        }
        else noSpawn = false;


        //balls and mouse area
        float randomX = Random.Range(-2f, 2f);
        Vector2 center = new Vector2(randomX, sBallPrefab.transform.position.y);
        sBallPrefab.transform.position = center;
        sMouseArea.transform.position = new Vector3(center.x, center.y, -0.01f);
        sMouseArea.GetComponent<SpriteRenderer>().enabled = true;
        MouseAreaManager.ballCenter = center;

        balls.Clear();
        nBalls++;
        for (int i = 0; i < nBalls * (doubleBalls ? 2 : 1); i++)
            balls.Add(Instantiate(sBallPrefab));
        nBallsBoard = balls.Count();
        ballMoving = false;
        doubleBalls = false;
    }

    public static void removeBlock(GameObject block) {
        blocks.Remove(block);
        objectsDepths.Remove(block);
    }

    public static void skipLevel() {
        foreach (GameObject ball in balls)
            Destroy(ball);
        nBallsBoard = 0;
        newLevel();
    }

    public static void turnSound() {
        sound = !sound;

        if (sound)
            sSoundObject.GetComponent<SpriteRenderer>().sprite = sSoundOn;

        else sSoundObject.GetComponent<SpriteRenderer>().sprite = sSoundOff;
    }

    public static void activatePickup(GameObject p) {
        Destroy(p);
        pickups.Remove(p);
        objectsDepths.Remove(p);

        string pickupName = p.GetComponent<SpriteRenderer>().sprite.name.Split('_')[0];

        switch (pickupName) {

            case "plus-one":
                nBalls++;
                break;

            case "plus-two":
                nBalls += 2;
                break;

            case "plus-three":
                nBalls += 3;
                break;

            case "bigger-pointer":
                MouseAreaManager.pointerLength = 3;
                break;

            case "clear-map":
                foreach (GameObject block in blocks) {
                    Destroy(block);
                    objectsDepths.Remove(block);
                }
                blocks.Clear();
                break;

            case "halve-blocks":
                foreach (GameObject block in blocks)
                    block.GetComponent<BlockManager>().halveHP();
                break;

            case "2x":
                doubleBalls = true;
                break;

            case "no-spawn":
                noSpawn = true;
                break;
        }
    }
}
