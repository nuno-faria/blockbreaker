using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager gm;

    public List<GameObject> blocksPrefabs;
    public List<int> blocksWeights;
    public GameObject ballPrefab;
    public GameObject mouseArea;
    public Text level;
    public GameObject soundObject;
    public Sprite soundOn;
    public Sprite soundOff;
    public GameObject pickupPrefab;
    public List<Sprite> pickupsSprites;
    public List<int> pickupsWeights;
    public int pickupWeight = 20; // prob spawning = (pickupWeight / 100) * 0.95
    public Text healthText;
    public SpriteRenderer healthIcon;
    public float ballSpeed = 18f;
    public List<int> blockSpawnWeights; // used after level 2

    private List<GameObject> balls;
    private List<GameObject> blocks;
    private List<GameObject> pickups;
    private Dictionary<GameObject, int> objectsDepths; //blocks and pickups
    private int nBallsBoard;
    private float blockSize;
    public int currentLevel = 0;
    public bool ballMoving = false;
    public bool sound;
    public int nBalls;
    private bool doubleBalls;
    private bool noSpawn;
    private int health;
    private bool doubleSpeed;
    public static bool classic = false;
    public static bool large = false;
    private Vector2 scaleVector;
    private int colMaxLimit;
    private int largeRowSize = 9;
    private ProbabilityPicker<int> numBlocksSpawnProbPicker;
    private ProbabilityPicker<GameObject> blockTypesProbPicker;
    private ProbabilityPicker<string> pickupsProbPicker;
    //TODO PICKUP LIMIT


    void Start () {
        gm = this;
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
        doubleSpeed = false;
        MouseAreaController.ballCenter = ballPrefab.transform.position;

        //health
        if (classic) {
            health = -1;
            healthText.text = "";
            healthIcon.enabled = false;
        }
        else {
            health = 1;
            healthText.text = "1";
        }

        //block spawn probabilities
        numBlocksSpawnProbPicker = new ProbabilityPicker<int>();
        for (int i = 0; i < blockSpawnWeights.Count; i++)
            numBlocksSpawnProbPicker.Add(i + 3, blockSpawnWeights[i]);

        //blocks type probabilities
        blockTypesProbPicker = new ProbabilityPicker<GameObject>();
        for (int i = 0; i < blocksPrefabs.Count; i++)
            blockTypesProbPicker.Add(blocksPrefabs[i], blocksWeights[i]);

        //pickups probabilities
        pickupsProbPicker = new ProbabilityPicker<string>();
        for (int i = 0; i < pickupsSprites.Count; i++)
            pickupsProbPicker.Add(pickupsSprites[i].name, pickupsWeights[i]);
        if (classic)
            pickupsProbPicker.Remove("heart_pickup");

        //large mode
        if (large) {
            scaleVector = new Vector2(7f / largeRowSize, 7f / largeRowSize);
            blockSize = blocksPrefabs[0].GetComponent<Renderer>().bounds.size.x * (7f/ largeRowSize);
            colMaxLimit = Mathf.FloorToInt(largeRowSize * 10f / 7f);
            balls[0].transform.localScale *= scaleVector;
        }
        else {
            scaleVector = new Vector2(1f, 1f);
            blockSize = blocksPrefabs[0].GetComponent<Renderer>().bounds.size.x;
            colMaxLimit = 10;
        }

        GenerateRow(Random.Range(2, 4));
    }


    //max nBlocks = 7
    private void GenerateRow(int nBlocks) {
        if (large)
            nBlocks = Mathf.RoundToInt(nBlocks * largeRowSize / 7f);

        List<int> l = RowIndexes();
        for (int i = 0; i < nBlocks; i++) {
            int idx = Random.Range(0, l.Count);
            CreateBlock(l[idx]);
            l.Remove(l[idx]);
        }

        //pickup
        if (l.Count > 0) {
            int x = Random.Range(0, 100);
            if (x < pickupWeight)
                CreatePickup(l[Random.Range(0,l.Count)]);
        }
    }


    List<int> RowIndexes() {
        if (!large)
            return new List<int>{ -3, -2, -1, 0, 1, 2, 3};
        else {
            List<int> l = new List<int>();
            l.Add(0);
            for (int i = 1; i <= (largeRowSize - 1) / 2; i++) {
                l.Add(i);
                l.Add(-i);
            }
            return l;
        }
    }


    void CreateBlock(int col) {
        GameObject prefab;
        if (classic)
            prefab = blocksPrefabs[0];
        else
            prefab = blockTypesProbPicker.Pick();

        GameObject block = Instantiate(prefab);
        block.transform.localScale *= scaleVector;
        block.transform.position = new Vector2(blockSize * col, 3.6f);
        blocks.Add(block);
        objectsDepths.Add(block, 1);
    }


    void CreatePickup(int col) {
        string sprite = pickupsProbPicker.Pick();
        GameObject pickup = Instantiate(pickupPrefab);
        pickup.GetComponent<SpriteRenderer>().sprite = pickupsSprites.Where(s => s.name == sprite).First();
        pickup.transform.position = new Vector2(blockSize * col, 3.6f);
        pickups.Add(pickup);
        objectsDepths.Add(pickup, 1);
        pickup.transform.localScale *= scaleVector;
    }


    public IEnumerator ThrowBalls(Vector2 dir) {
        ballMoving = true;
        mouseArea.GetComponent<SpriteRenderer>().enabled = false;
        foreach (GameObject b in balls.ToList()) {
            b.GetComponent<Rigidbody2D>().velocity = dir * ballSpeed * (doubleSpeed ? 2 : 1);
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
        CheckGameOver();
        MoveObjects();
        SpawnRow();
        ResetThrowArea();
    }


    void CheckGameOver() {            
        if (objectsDepths.Any(x => x.Value == colMaxLimit)) {
            //update health
            if (!classic)
                UpdateHealth(-1);

            if (health < 0)
                SceneManager.LoadScene("GameOverScene");
            else
                ClearLastRow();
        }
    }


    void UpdateHealth(int inc) {
        health += inc;
        healthText.text = health.ToString();
    }


    void ClearLastRow() {
        foreach (var block in objectsDepths.Where(x => x.Value == colMaxLimit).Select(x => x.Key).ToList()) {
            RemoveBlock(block);
            Destroy(block);
        }
    }


    void MoveObjects() {
        //destroy pickup if depth is 6
        foreach (GameObject pickup in pickups.ToList()) {
            if (objectsDepths[pickup] == 6) {
                objectsDepths.Remove(pickup);
                pickups.Remove(pickup);
                Destroy(pickup);
            }
        }

        //translate blocks
        foreach (GameObject block in blocks) {
            Vector3 rotation = block.transform.eulerAngles;
            block.transform.eulerAngles = Vector3.zero;
            block.transform.Translate(new Vector2(0, -blockSize));
            block.transform.eulerAngles = rotation;
        }

        //translate pickups
        foreach (GameObject pickup in pickups)
            pickup.transform.Translate(new Vector2(0, -blockSize));

        //update depth
        objectsDepths.Keys.ToList().ForEach(x => objectsDepths[x] += 1);
    }


    void SpawnRow() {
        if (!noSpawn) {
            if (currentLevel < 20 && !noSpawn)
                GenerateRow(Random.Range(3, 6));
            else
                GenerateRow(numBlocksSpawnProbPicker.Pick());
        }
        else
            noSpawn = false;
    }


    void ResetThrowArea() {
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
        balls.ForEach(x => x.transform.localScale *= scaleVector);
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

            case "heart":
                UpdateHealth(+1);
                break;

            case "2x-speed":
                doubleSpeed = true;
                pickupsProbPicker.Remove("2x-speed_pickup");
                break;
        }
    }
}
