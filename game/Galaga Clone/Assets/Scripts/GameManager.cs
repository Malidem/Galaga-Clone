﻿using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject backgroundsFolder;
    public GameObject bulletFolder;
    public GameObject enemyFolder;
    public GameObject turretBulletFolder;
    public GameObject canvas;
    public GameObject player;
    public GameObject gameOverMenu;
    public GameObject HUD;
    public GameObject backgroundPrefab;
    public GameObject explosion;
    public GameObject dialogueBackground;
    public GameObject bossSpawnPoint;
    public GameObject enemySpawnPointsFolder;
    public GameObject startMenu;
    public Image overheatBar;
    public Text dialogueText;
    public Texture2D cursor;
    public AudioClip overheatSound;
    public Slider bossHealthBar;
    public List<Sprite> overheatImagesL0 = new List<Sprite>();
    public List<Sprite> overheatImagesL1 = new List<Sprite>();
    public List<Sprite> overheatImagesL2 = new List<Sprite>();
    public EnemySpawnProperties[] spawnList;

    [HideInInspector]
    public bool gameStarted;
    [HideInInspector]
    public bool gameOver;
    [HideInInspector]
    public Transform[] HUDElements;
    [HideInInspector]
    public bool overheatCooldown;
    [HideInInspector]
    public bool gamePaused;
    [HideInInspector]
    public AudioSource audioSource;
    [HideInInspector]
    public int overheatAmount;
    [HideInInspector]
    public int overheatMax = 100;
    [HideInInspector]
    public int gunLevel = 0;
    [HideInInspector]
    public RectTransform backgroundRect;
    [HideInInspector]
    public List<Transform> enemySpawnPoints;

    private int money;
    private int wave;
    private int waveCount;
    private List<string> dialogues = new List<string>();
    private List<float> dialogueIntervals = new List<float>();
    private List<float> waveIntervals = new List<float>();
    private List<GameObject> enemyCount = new List<GameObject>();
    private bool canPlayOverheatSound = true;
    private bool playerWon;
    private bool isFinalWave;
    private Texture2D levelMap;
    private readonly string levelPath = Application.streamingAssetsPath + "/Levels/level_" + Constants.levelsUnlocked;

    void OnEnable()
    {
        Button[] allButtons = canvas.GetComponentsInChildren<Button>(true);
        foreach (Button button in allButtons)
        {
            button.GetComponent<Button>().onClick.AddListener(delegate { Constants.dontDestroy.GetComponent<ButtonAudio>().PlayClickSound(); });
        }
    }

    void Start()
    {
        print("Loading level " + Constants.levelsUnlocked);
        backgroundRect = (RectTransform)backgroundsFolder.transform;
        HUDElements = HUD.GetComponentsInChildren<Transform>();
        startMenu.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "Level " + Constants.levelsUnlocked;
        StartCoroutine(Overheat());
        Instantiate(backgroundPrefab, backgroundsFolder.transform.position, transform.rotation, backgroundsFolder.transform);
        Cursor.SetCursor(cursor, Vector3.zero, CursorMode.ForceSoftware);
        audioSource = canvas.GetComponent<AudioSource>();
        audioSource.volume = PlayerPrefs.GetFloat(Constants.Prefs.soundVolume);
        ReadLevelProperties();
        var rawImage = File.ReadAllBytes(levelPath + ".png");
        levelMap = new Texture2D(2, 2);
        levelMap.LoadImage(rawImage);
    }

    private void ReadLevelProperties()
    {
        StreamReader streamReader = new StreamReader(levelPath + ".txt");
        string contents = streamReader.ReadToEnd();
        streamReader.Close();

        int dialogueCount = 0;
        List<string> lines = new List<string>(contents.Split('\n'));

        string[] line0 = Break(lines[0].Replace(" ", ""));
        CheckFileVariable(line0[0], "waveCount", delegate { waveCount = int.Parse(line0[1]); });

        string[] line1 = Break(lines[1]);
        CheckFileVariable(line1[0], "waveIntervals", delegate {
            string[] values = line1[1].Split(',');

            for (int i = 0; i < values.Length; i++)
            {
                waveIntervals.Add(float.Parse(values[i]));
            }
        });

        string[] line2 = Break(lines[2].Replace(" ", ""));
        CheckFileVariable(line2[0], "dialogueCount", delegate { dialogueCount = int.Parse(line2[1]); });

        for (int i = 0; i < dialogueCount; i++)
        {
            print("Reading dialogue block " + i);
            int blockStart = IndexOfContains(lines, "dialogueStart:");
            int blockEnd = IndexOfContains(lines, "dialogueEnd:") - blockStart;
            List<string> dialogueBlock = lines.GetRange(blockStart, blockEnd);
            lines.RemoveRange(blockStart, blockEnd + 1);

            string[] intervalLine = Break(dialogueBlock[1].Replace(" ", ""));
            CheckFileVariable(intervalLine[0], "interval", delegate { dialogueIntervals.Add(float.Parse(intervalLine[1])); });

            int dialogueTextStart = IndexOfContains(dialogueBlock, "<<<");
            dialogues.Add(string.Join("", dialogueBlock.GetRange(dialogueTextStart, IndexOfContains(dialogueBlock, ">>>") - dialogueTextStart)).TrimStart('<'));
        }
    }

    private void CheckFileVariable(string line, string validLine, Action action)
    {
        if (line == validLine)
        {
            action();
        }
        else
        {
            Debug.LogError("Invalid file variable (" + line + ")");
        }
    }

    private string[] Break(string line)
    {
        return line.Split('=');
    }

    private int IndexOfContains(List<string> list, string str)
    {
        int result = -1;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Contains(str))
            {
                result = i;
                break;
            }
        }
        return result;
    }

    public void StartWaves()
    {
        StartCoroutine(UpdateWaves());
    }

    private IEnumerator UpdateWaves()
    {
        int yPixelsPerChunk = 13;
        int xPixelsPerChunk = 10;
        int xNumberOfChunks = levelMap.width / xPixelsPerChunk;
        int yNumberOfChunks = levelMap.height / yPixelsPerChunk;

        for (int chunkY = 0; chunkY < yNumberOfChunks; chunkY++)
        {
            for (int chunkX = 0; chunkX < xNumberOfChunks; chunkX++)
            {
                if (gameOver == false && wave <= (waveCount - 1))   // waveCount - 1 because waveCount dosn't count 0 as 1
                {
                    yield return new WaitForSeconds(waveIntervals[wave]);
                    print("Starting wave " + wave);
                    Transform g = Instantiate(enemySpawnPointsFolder, enemyFolder.transform).transform;
                    enemySpawnPoints = new List<Transform>(g.GetComponentsInChildren<Transform>());
                    enemySpawnPoints.Remove(g);
                    for (int y = 0; y < yPixelsPerChunk; y++)
                    {
                        for (int x = 0; x < xPixelsPerChunk; x++)
                        {
                            int pixelX = (chunkX * xPixelsPerChunk) + x;
                            int pixelY = (chunkY * yPixelsPerChunk) + y;
                            PixelToEnemy(levelMap, pixelX, pixelY, x, y);
                        }
                    }
                    enemySpawnPoints.Clear();
                    Destroy(g.gameObject);
                    wave++;
                    if (wave > (waveCount - 1))
                    {
                        isFinalWave = true;
                    }
                }
                else
                {
                    yield break;
                }
            }
        }
    }

    private void PixelToEnemy(Texture2D image, int imageX, int imageY, int chunkX, int chunkY)
    {
        Color pixelColor = image.GetPixel(imageX, imageY);

        if (pixelColor.a == 0)
        {
            return;
        }

        foreach (EnemySpawnProperties spawnProps in spawnList)
        {
            if (spawnProps.color.Equals(pixelColor) && spawnProps.isBoss)
            {
                GameObject bossEnemy = Instantiate(spawnProps.enemyType, bossSpawnPoint.transform.position, Quaternion.identity, enemyFolder.transform);
                enemyCount.Add(bossEnemy);
                BaseEnemy bossProps = bossEnemy.GetComponent<BaseEnemy>();
                bossProps.canFireGuns = false;
                bossProps.canFireTurrets = false;
                bossProps.canTakeDamage = false;
                bossProps.canShieldsTakeDamage = false;
            }
            else if (spawnProps.color.b.Equals(pixelColor.b) && spawnProps.color.g.Equals(pixelColor.g))
            {
                for (int i = 0; i < enemySpawnPoints.Count; i++)
                {
                    EnemySpawnPoint spawnPoint = enemySpawnPoints[i].gameObject.GetComponent<EnemySpawnPoint>();
                    if (spawnPoint.pixelX == chunkX && spawnPoint.pixelY == chunkY)
                    {
                        GameObject enemy = Instantiate(spawnProps.enemyType, spawnPoint.gameObject.transform.position, Quaternion.Euler(0, 0, pixelColor.r * 255 * 5), enemyFolder.transform);
                        enemyCount.Add(enemy);
                    }
                }
            }
        }
    }

    void Update()
    {
        if (gameStarted && gameOver == false)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (Time.timeScale == 0)
                {
                    Time.timeScale = 1;
                    pauseMenu.SetActive(false);
                    gamePaused = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                else
                {
                    Time.timeScale = 0;
                    pauseMenu.SetActive(true);
                    gamePaused = true;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
        }
    }

    public void AddMoney(int amount)
    {
        money += amount;
        HUDElements[2].gameObject.GetComponent<Text>().text = "Credits: " + string.Format("{0:n0}", money);
    }

    private IEnumerator Overheat()
    {
        while (gameOver == false)
        {
            yield return new WaitForSeconds(0.15F);
            if (overheatAmount > (overheatMax - 5))
            {
                overheatCooldown = true;
            }
            if (overheatAmount > 0)
            {
                if (overheatCooldown)
                {
                    if (canPlayOverheatSound)
                    {
                        yield return new WaitForSeconds(1F);
                        audioSource.PlayOneShot(overheatSound);
                        canPlayOverheatSound = false;
                    }
                    overheatAmount -= 5;
                    UpdateOverheatSprite();
                    if (overheatAmount <= 0)
                    {
                        overheatCooldown = false;
                        canPlayOverheatSound = true;
                    }
                }
                else
                {
                    overheatAmount -= 1;
                    UpdateOverheatSprite();
                }
            }
        }
    }

    public void UpdateOverheatSprite()
    {
        Image overheatImage = overheatBar.GetComponent<Image>();
        double index = Math.Round(overheatAmount / (overheatMax / 8D), 0);  // 8 is the number of images not including 0
        if (gunLevel == 0)
        {
            overheatImage.sprite = overheatImagesL0[(int)index];
        }
        else if (gunLevel == 1)
        {
            overheatImage.sprite = overheatImagesL1[(int)index];
        }
        else if (gunLevel == 2)
        {
            overheatImage.sprite = overheatImagesL2[(int)index];
        }
    }

    public void EndGame()
    {
        Transform[] menu = gameOverMenu.GetComponentsInChildren<Transform>(true);
        gameOver = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gameOverMenu.SetActive(true);
        HUDElements[2].gameObject.SetActive(false);
        
        if (playerWon)
        {
            print("Level completed!");
            menu[3].gameObject.SetActive(true);
            menu[3].GetComponent<Text>().text = "Credits earned: " + string.Format("{0:n0}", money);
            menu[2].GetComponent<Text>().text = "Level Completed!";
            menu[4].GetChild(0).GetComponent<Text>().text = "Next Level";
            Constants.money += money;
            Constants.levelsUnlocked++;
            Constants.levelsCompleted++;
            string[] shopItems = { "none", "none", "none" };
            Constants.shopItems = shopItems;
            StartCoroutine(Constants.SaveDataToDatabase());
        }
        else
        {
            print("Game Over!");
        }
    }

    public void Kill(GameObject gameObject, float scale)
    {
        StartCoroutine(KillWithExplosion(gameObject, scale));
    }

    private IEnumerator KillWithExplosion(GameObject gameObject, float scale)
    {
        if (gameObject.transform.position.x > -25)
        {
            int rotation = UnityEngine.Random.Range(0, 180);
            GameObject explosionGO = Instantiate(explosion, gameObject.transform.position, Quaternion.AngleAxis(rotation, Vector3.forward), gameObject.transform);
            explosionGO.transform.localScale = new Vector2(scale, scale);
            explosionGO.GetComponent<Explosion>().parent = gameObject;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            if (gameObject.name != "Player")
            {
                gameObject.GetComponent<BaseEnemy>().canFireGuns = false;
                gameObject.GetComponent<BaseEnemy>().canFireTurrets = false;
                OnEnemyDestroyed(gameObject);
            }
            yield return explosionGO.GetComponent<Explosion>().Die();
        }
        Destroy(gameObject);
    }

    public void OnEnemyDestroyed(GameObject enemy)
    {
        if (gameOver == false)
        {
            if (!enemy.CompareTag("BossEnemy"))
            {
                enemyCount.Remove(enemy);
                if (isFinalWave && enemyCount.Count <= 0)
                {
                    playerWon = true;
                    EndGame();
                }
            }
            else
            {
                playerWon = true;
                EndGame();
            }
        }
    }

    public void CallUpdateDialogue()
    {
        StartCoroutine(UpdateDialogue());
    }

    private IEnumerator UpdateDialogue()
    {
        for (int i = 0; i < dialogues.Count; i++)
        {
            yield return new WaitForSeconds(dialogueIntervals[i]);
            if (gameOver == false)
            {
                string text = dialogues[i];
                RectTransform rect = dialogueBackground.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(0, rect.sizeDelta.y);
                dialogueBackground.SetActive(true);
                dialogueText.gameObject.SetActive(true);

                float i2 = text.Length;
                while (i2 >= 0)
                {
                    if (rect.sizeDelta.x <= 555)
                    {
                        rect.sizeDelta = new Vector2(rect.sizeDelta.x + 15, rect.sizeDelta.y);
                    }
                    else
                    {
                        break;
                    }
                    yield return new WaitForSeconds(0.05F);
                    i2--;
                }

                yield return new WaitForSeconds(0.5F);

                int max = 148; // This is the max number of chars in a chunk/page of dialogue
                if ((text.Length + 1) > max)
                {
                    int start = 1; // This is the starting index for the current chunk/page being displayed
                    int end = start + max; // This is the ending index of the current chunk/page being displayed
                    do
                    {
                        if (gameOver == false)
                        {
                            for (; end > start; end--)
                            {
                                if (end > (text.Length + 1))
                                {
                                    end = (text.Length - start);
                                    break;
                                }
                                else if (text.Substring(end, 1) == " ")
                                {
                                    end -= start;
                                    break;
                                }
                            }
                            dialogueText.text = text.Substring(start, end);
                            start += end + 1;
                            end = start + max;
                            yield return new WaitForSeconds(5);
                        }
                        else
                        {
                            break;
                        }
                    } while (start < text.Length);
                }
                else
                {
                    dialogueText.text = text;
                    yield return new WaitForSeconds(5);
                }

                dialogueText.text = "";
                dialogueText.gameObject.SetActive(false);

                int i3 = 0;
                while (i3 <= rect.sizeDelta.x)
                {
                    rect.sizeDelta = new Vector2(rect.sizeDelta.x - 15, rect.sizeDelta.y);
                    yield return new WaitForSeconds(0.05F);
                    i3++;
                }

                dialogueBackground.SetActive(false);
            }
            else
            {
                yield break;
            }
        }
    }

    [Serializable]
    public class EnemySpawnProperties
    {
        public GameObject enemyType;
        public Color color;
        public bool isBoss;
    }
}
