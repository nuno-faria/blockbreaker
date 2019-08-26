using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager gm;

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


    private List<GameObject> balls;
    private List<GameObject> blocks;
    private Dictionary<GameObject, int> objectsDepths; //blocks and pickups
    private List<GameObject> pickups;
    private int nBallsBoard;
    private float blockSize;
    public int currentLevel = 0;
    public bool ballMoving = false;
    public bool sound;
    public int nBalls;
    private bool doubleBalls;
    private bool noSpawn;


    void Start () {
        gm = this;


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

        MouseAreaController.ballCenter = ballPrefab.transform.position;

        GenerateRow(Random.Range(2, 4));
    }


    //max nBlocks = 7
    private void GenerateRow(int nBlocks) {
        List<int> l = new List<int> { -3, -2, -1, 0, 1, 2, 3};
        for (int i = 0; i< nBlocks; i++) {
            int idx = Random.Range(0, l.Count);
            CreateBlock(l[idx]);
            l.Remove(l[idx]);
        }

        //pickup
        if (l.Count > 0) {
            int x = Random.Range(0, 100);
            if (x < 15)
                CreatePickup(l[Random.Range(0,l.Count)]);
        }
    }


    private void CreateBlock(int col) {
        GameObject g = Instantiate(blockPrefab);
        g.transform.position = new Vector2(blockSize * col, 3.6f);
        blocks.Add(g);
        objectsDepths.Add(g, 1);
    }


    private void CreatePickup(int col) {
        string sprite = "";
        //TODO
        int x = Random.Range(0, 88);
        if (x < 20) sprite = "plus-one";
        else if (x >= 20 && x < 40) sprite = "plus-two";
        else if (x >= 40 && x < 50) sprite = "plus-three";
        else if (x >= 50 && x < 68) sprite = "2x";
        else if (x >= 68 && x < 80) sprite = "no-spawn";
        else if (x >= 80 && x < 86) sprite = "halve-blocks";
        else if (x >= 86 && x < 88) sprite = "clear-map";

        sprite += "_pickup";

        GameObject g = Instantiate(pickupPrefab);
        g.GetComponent<SpriteRenderer>().sprite = pickupsSprites.Where(s => s.name == sprite).First();
        g.transform.position = new Vector2(blockSize * col, 3.6f);
        pickups.Add(g);
        objectsDepths.Add(g, 1);
    }


    public IEnumerator ThrowBalls(Vector2 dir) {
        ballMoving = true;
        mouseArea.GetComponent<SpriteRenderer>().enabled = false;
        foreach (GameObject b in balls.ToList()) {
            b.GetComponent<Rigidbody2D>().velocity = dir * 18;
            yield return new WaitForSeconds(0.07f);
        }
    }


    public void DecrementNBallsBoard() {
        nBallsBoard--;
        if (nBallsBoard == 0)
            NewLevel();
    }


    public void NewLevel() {
        currentLevel++;
        level.text = currentLevel.ToString();
            

        //check if game over
        foreach (GameObject block in objectsDepths.Keys.Where(x => blocks.Contains(x)).ToList()) {
            if (objectsDepths[block] == 10)
                SceneManager.LoadScene("GameOverScene");
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
                GenerateRow(Random.Range(3, 6));
            else {
                int x = Random.Range(0, 100);
                if (x < 5) GenerateRow(3);
                else if (x >= 5 && x < 35) GenerateRow(4);
                else if (x >= 35 && x < 80) GenerateRow(5);
                else if (x >= 80 && x < 95) GenerateRow(6);
                else if (x >= 95) GenerateRow(7);
            }
        }
        else noSpawn = false;


        //balls and mouse area
        float randomX = Random.Range(-2f, 2f);
        Vector2 center = new Vector2(randomX, ballPrefab.transform.position.y);
        ballPrefab.transform.position = center;
        mouseArea.transform.position = new Vector3(center.x, center.y, -0.01f);
        mouseArea.GetComponent<SpriteRenderer>().enabled = true;
        MouseAreaController.ballCenter = center;

        balls.Clear();
        nBalls++;
        for (int i = 0; i < nBalls * (doubleBalls ? 2 : 1); i++)
            balls.Add(Instantiate(ballPrefab));
        nBallsBoard = balls.Count();
        ballMoving = false;
        doubleBalls = false;
    }


    public void RemoveBlock(GameObject block) {
        blocks.Remove(block);
        objectsDepths.Remove(block);
    }


    public void SkipLevel() {
        foreach (GameObject ball in balls)
            Destroy(ball);
        nBallsBoard = 0;
        NewLevel();
    }


    public void TurnSound() {
        sound = !sound;

        if (sound)
            soundObject.GetComponent<SpriteRenderer>().sprite = soundOn;

        else soundObject.GetComponent<SpriteRenderer>().sprite = soundOff;
    }


    public void ActivatePickup(GameObject p) {
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

            case "clear-map":
                foreach (GameObject block in blocks) {
                    Destroy(block);
                    objectsDepths.Remove(block);
                }
                blocks.Clear();
                break;

            case "halve-blocks":
                foreach (GameObject block in blocks)
                    block.GetComponent<BlockController>().halveHP();
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
