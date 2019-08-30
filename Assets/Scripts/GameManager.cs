using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class GameManager : MonoBehaviour {

    public static GameManager gm;

    public List<GameObject> blocksPrefabs;
    public List<int> blocksWeights;
    public GameObject ballPrefab;
    public GameObject mouseArea;
    private SpriteRenderer mouseAreaSprite;
    public Text levelText;
    public GameObject soundObject;
    private SpriteRenderer soundSpriteRenderer;
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
    public ParticleSystem fireworkParticleSystem;
    public Text nBallsText;
    public GameObject saveButton;

    private List<GameObject> balls;
    private List<GameObject> blocks;
    private List<GameObject> pickups;
    private Dictionary<GameObject, int[]> objectsPositions; //blocks and pickups, [x,y]
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
    public static bool hardcore = false;
    public static bool continueGame = false;
    private Vector2 scaleVector;
    private int colMaxLimit;
    private int largeRowSize = 9;
    private int pickupLimit;
    private ProbabilityPicker<int> numBlocksSpawnProbPicker;
    private ProbabilityPicker<GameObject> blockTypesProbPicker;
    private ProbabilityPicker<string> pickupsProbPicker;

    private string gameMode;


    void Start () {
        gm = this;
        balls = new List<GameObject>();
        ballPrefab.transform.position = new Vector2(0, -4.3f);
        blocks = new List<GameObject>();
        pickups = new List<GameObject>();
        objectsPositions = new Dictionary<GameObject, int[]>();
        nBalls = 0;
        nBallsBoard = 0;
        currentLevel = 0;
        ballMoving = false;
        levelText.text = "1";
        sound = true;
        doubleBalls = false;
        doubleSpeed = false;
        mouseAreaSprite = mouseArea.GetComponent<SpriteRenderer>();
        soundSpriteRenderer = soundObject.GetComponent<SpriteRenderer>();
        nBallsText.text = "1";
        MouseAreaController.ballCenter = ballPrefab.transform.position;

        //health
        if (classic || hardcore) {
            health = -1;
            healthText.text = "";
            healthIcon.enabled = false;
        }
        else {
            health = 1;
            healthText.text = "1";
        }

        //large mode
        if (large) {
            scaleVector = new Vector2(7f / largeRowSize, 7f / largeRowSize);
            blockSize = blocksPrefabs[0].GetComponent<Renderer>().bounds.size.x * (7f / largeRowSize);
            colMaxLimit = Mathf.FloorToInt(largeRowSize * 10f / 7f);
            pickupLimit = Mathf.RoundToInt(colMaxLimit * 6f / 10f);
        }
        else {
            scaleVector = new Vector2(1f, 1f);
            blockSize = blocksPrefabs[0].GetComponent<Renderer>().bounds.size.x;
            colMaxLimit = 10;
            pickupLimit = 6;
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
        
        //gamemode
        if (classic)
            gameMode = "classic";
        else if (large)
            gameMode = "large";
        else if (hardcore)
            gameMode = "hardcore";
        else
            gameMode = "modern";

        if (!continueGame)
            NewLevel();
        else
            LoadState();
    }


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
        if (!hardcore && l.Count > 0) {
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


    void CreateBlock(int col, int row = 0, int? idx = null, int? hp = null) {
        GameObject prefab;
        if (idx != null)
            prefab = blocksPrefabs[idx.Value];
        else if (classic)
            prefab = blocksPrefabs[0];
        else
            prefab = blockTypesProbPicker.Pick();

        GameObject block = Instantiate(prefab);
        block.transform.localScale *= scaleVector;
        block.transform.position = new Vector2(blockSize * col, 3.6f - blockSize * row);
        if (hp != null) {
            var controller = block.GetComponent<BlockController>();
            controller.hp = hp.Value;
            controller.UpdateHP();
        }
        blocks.Add(block);
        objectsPositions.Add(block, new int[2]{ col, row });
    }


    void CreatePickup(int col, int row = 0, int? idx = null) {
        Sprite sprite;
        if (idx != null)
            sprite = pickupsSprites[idx.Value];
        else {
            string spriteName = pickupsProbPicker.Pick();
            sprite = pickupsSprites.Where(s => s.name == spriteName).First();
        }

        GameObject pickup = Instantiate(pickupPrefab);
        pickup.GetComponent<SpriteRenderer>().sprite = sprite;
        pickup.transform.localScale *= scaleVector;
        pickup.transform.position = new Vector2(blockSize * col, 3.6f - blockSize * row);
        pickups.Add(pickup);
        objectsPositions.Add(pickup, new int[2] { col, row });
    }


    public void DecrementNBallsBoard() {
        nBallsBoard--;
        if (nBallsBoard == 0)
            NewLevel();
    }


    public void NewLevel() {
        currentLevel++;
        levelText.text = currentLevel.ToString();
        nBalls++;
        CheckGameOver();
        MoveObjects();
        SpawnRow();
        ResetThrowArea();
        if (currentLevel % 10 == 0)
            LaunchFirework();
        UpdateHighscore();
        saveButton.SetActive(true);
    }


    void CheckGameOver() {
        if (objectsPositions.Any(x => x.Value[1] == colMaxLimit - 1)) {
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
        foreach (var block in objectsPositions.Where(x => x.Value[1] == colMaxLimit - 1).Select(x => x.Key).ToList()) {
            RemoveBlock(block);
            Destroy(block);
        }
    }


    void MoveObjects() {
        //destroy pickup if depth is 6
        foreach (GameObject pickup in pickups.ToList()) {
            if (objectsPositions[pickup][1] == pickupLimit - 1) {
                objectsPositions.Remove(pickup);
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
        objectsPositions.Keys.ToList().ForEach(x => objectsPositions[x][1] += 1);
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
        mouseAreaSprite.enabled = true;
        MouseAreaController.ballCenter = center;

        balls.Clear();
        for (int i = 0; i < nBalls * (doubleBalls ? 2 : 1); i++)
            balls.Add(Instantiate(ballPrefab));
        balls.ForEach(x => x.transform.localScale *= scaleVector);
        nBallsBoard = balls.Count();
        ballMoving = false;
        nBallsText.text = balls.Count.ToString();
        nBallsText.enabled = true;
    }


    void LaunchFirework() {
        var firework = Instantiate(fireworkParticleSystem);
        firework.transform.position = levelText.transform.position;
    }


    public void RemoveBlock(GameObject block) {
        blocks.Remove(block);
        objectsPositions.Remove(block);
    }


    public void ActivatePickup(GameObject p) {
        Destroy(p);
        pickups.Remove(p);
        objectsPositions.Remove(p);

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

            case "clear-map-100":
                RemoveBlocks(100);
                break;

            case "clear-map-50":
                RemoveBlocks(50);
                break;

            case "clear-map-15":
                RemoveBlocks(15);
                break;
        }
    }


    void RemoveBlocks(int percentage) {
        int blocksToRemove = Mathf.RoundToInt(blocks.Count * percentage / 100f);
        for (int i=0; i<blocksToRemove; i++) {
            var block = blocks[Random.Range(0, blocks.Count)];
            Destroy(block);
            RemoveBlock(block);
        }
    }


    public IEnumerator ThrowBalls(Vector2 dir) {
        ballMoving = true;
        nBallsText.enabled = false;
        mouseAreaSprite.enabled = false;
        doubleBalls = false;
        saveButton.SetActive(false);
        foreach (GameObject b in balls.ToList()) {
            b.GetComponent<Rigidbody2D>().velocity = dir * ballSpeed * (doubleSpeed ? 2 : 1);
            yield return new WaitForSeconds(0.07f);
        }
    }


    public void SkipLevel() {
        foreach (GameObject ball in balls)
            Destroy(ball);
        nBallsBoard = 0;
        NewLevel();
    }


    public void ToggleSound() {
        sound = !sound;
        if (sound)
            soundSpriteRenderer.sprite = soundOn;
        else
            soundSpriteRenderer.sprite = soundOff;
    }


    //STATE

    void UpdateHighscore() {
        int currentHighscore = 1;

        if (PlayerPrefs.HasKey(gameMode + "_highscore"))
            currentHighscore = PlayerPrefs.GetInt(gameMode + "_highscore");

        if (currentLevel > currentHighscore)
            PlayerPrefs.SetInt(gameMode + "_highscore", currentLevel);
    }


    public void SaveState() {
        Dictionary<string, object> state = new Dictionary<string, object> {
            { "currentLevel", currentLevel },
            { "nBalls", nBalls },
            { "doubleBalls", doubleBalls },
            { "doubleSpeed", doubleSpeed },
            { "health", health },
            { "objects", new List<Dictionary<string, object>>() }
        };

        foreach (var keypair in objectsPositions) {
            Dictionary<string, object> obj = new Dictionary<string, object>();
            obj["type"] = ObjectType(keypair.Key);
            obj["idx"] = ObjectIdx(keypair.Key);
            obj["x"] = keypair.Value[0];
            obj["y"] = keypair.Value[1];
            obj["hp"] = (string) obj["type"] == "pickup" ? 0 : keypair.Key.GetComponent<BlockController>().hp;
            ((List<Dictionary<string, object>>) state["objects"]).Add(obj);
        }

        string json = JsonConvert.SerializeObject(state);
        PlayerPrefs.SetString("data", json);
        PlayerPrefs.SetString("mode", gameMode);
    }


    void LoadState() {
        gameMode = PlayerPrefs.GetString("mode");
        string json = PlayerPrefs.GetString("data");
        Dictionary<string, object> state = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        currentLevel = (int)(long) state["currentLevel"];
        nBalls = (int)(long) state["nBalls"];
        doubleBalls = (bool) state["doubleBalls"];
        doubleSpeed = (bool) state["doubleSpeed"];
        health = (int)(long) state["health"];

        var listObjects = (Newtonsoft.Json.Linq.JArray) state["objects"];
        foreach (var obj in listObjects) {
            if (((string)obj["type"]) == "pickup")
                CreatePickup((int)(long)obj["x"], (int)(long)obj["y"], (int)(long)obj["idx"]);
            else
                CreateBlock((int)(long)obj["x"], (int)(long)obj["y"], (int)(long)obj["idx"], (int)(long)obj["hp"]);
        }

        if (doubleSpeed)
            pickupsProbPicker.Remove("2x-speed");
        levelText.text = currentLevel.ToString();
        if (gameMode != "classic" && gameMode != "hardcore")
            healthText.text = health.ToString();
        nBalls *= doubleBalls ? 2 : 1;
        nBallsText.text = nBalls.ToString();
        balls.Clear();
        for (int i = 0; i < nBalls; i++)
            balls.Add(Instantiate(ballPrefab));
        nBallsBoard = nBalls;
    }


    private Dictionary<string, int> blocksIds;
    private Dictionary<string, int> pickupsIds;


    string ObjectType(GameObject obj) {
        return obj.tag == "pickup" ? "pickup" : "block";
    }


    int ObjectIdx(GameObject obj) {
        if (blocksIds == null) {
            blocksIds = new Dictionary<string, int>();
            for (int i = 0; i < blocksPrefabs.Count; i++)
                blocksIds[blocksPrefabs[i].name] = i;
        }

        if (pickupsIds == null) {
            pickupsIds = new Dictionary<string, int>();
            for (int i = 0; i < pickupsSprites.Count; i++)
                pickupsIds[pickupsSprites[i].name] = i;
        }


        if (ObjectType(obj) == "pickup")
            return pickupsIds[obj.GetComponent<SpriteRenderer>().sprite.name];
        else
            return blocksIds[obj.name.Replace("(Clone)", "")];
    }
}
