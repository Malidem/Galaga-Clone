using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject enemyType1;
    public GameObject background;
    public GameObject canvas;
    public GameObject player;
    public GameObject gameOverMenu;
    public GameObject HUD;
    public GameObject enemies;
    public GameObject backgroundPrefab;
    public GameObject explosion;
    public GameObject dialogueBackground;
    public Image overheatBar;
    public Sprite overheatOverlay0;
    public Sprite overheatOverlay1;
    public Sprite overheatOverlay2;
    public Sprite overheatOverlay3;
    public Sprite overheatOverlay4;
    public Sprite overheatOverlay5;
    public Sprite overheatOverlay6;
    public Sprite overheatOverlay7;
    public Sprite overheatOverlay8;
    public Text dialogueText;
    [TextArea(3, 10)] [SerializeField] protected List<string> dialogues = new List<string>();
    public List<float> timeBetweenDialogues = new List<float>();
    public Texture2D cursor;
    public int money;
    public AudioClip overheatSound;
    public List<GameObject> enemyTypes;
    public List<Sprite> overheatImagesL0 = new List<Sprite>();
    public List<Sprite> overheatImagesL1 = new List<Sprite>();
    public List<Sprite> overheatImagesL2 = new List<Sprite>();

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
    public int gunLevel = 1; // Level is 1 more than ingame level

    private int wave;
    private int waveAmount = 1;
    private bool canPlayOverheatSound = true;

    void Start()
    {
        Time.timeScale = 1;
        HUDElements = HUD.GetComponentsInChildren<Transform>();
        StartCoroutine(Overheat());
        Instantiate(backgroundPrefab, background.transform.position, transform.rotation, canvas.GetComponentsInChildren<Transform>()[1]);
        Cursor.SetCursor(cursor, Vector3.zero, CursorMode.ForceSoftware);
        audioSource = canvas.GetComponent<AudioSource>();
        audioSource.volume = PlayerPrefs.GetFloat("soundVolume");
    }

    public void StartWaves()
    {
        StartCoroutine(UpdateWave());
    }

    private IEnumerator UpdateWave()
    {
        while (gameOver == false)
        {
            if (waveAmount >= 5)
            {
                waveAmount += UnityEngine.Random.Range(1, 6);
            }
            else
            {
                waveAmount++;
            }
            wave++;
            HUDElements[3].gameObject.GetComponent<Text>().text = "Wave " + wave;
            SpawnEnemies();
            yield return new WaitForSeconds(8.0F);
        }
    }

    public void SpawnEnemies()
    {
        for (int i = 0; i < waveAmount; i++)
        {
            RectTransform rect = (RectTransform)background.transform;
            int pos = UnityEngine.Random.Range(1, 4);
            GameObject enemy = enemyTypes[UnityEngine.Random.Range(0, enemyTypes.Count)];
            if (pos == 1)
            {
                Instantiate(enemy, new Vector2(rect.rect.width + 25, UnityEngine.Random.Range(0, rect.rect.height)), transform.rotation, enemies.transform);
            }
            else if (pos == 2)
            {
                Instantiate(enemy, new Vector2(UnityEngine.Random.Range((rect.rect.width / 3) * 2, rect.rect.width + 25), rect.rect.height + 25), transform.rotation, enemies.transform);
            }
            else if (pos == 3)
            {
                Instantiate(enemy, new Vector2(UnityEngine.Random.Range((rect.rect.width / 3) * 2, rect.rect.width + 25), -25), transform.rotation, enemies.transform);
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
        HUDElements[2].gameObject.GetComponent<Text>().text = "Money: " + String.Format("{0:n0}", money);
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
                    overheatAmount -= 5;
                    UpdateOverheatSprite();
                    if (canPlayOverheatSound)
                    {
                        audioSource.PlayOneShot(overheatSound);
                        canPlayOverheatSound = false;
                    }
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
        if (gunLevel == 0)
        {
            if (overheatAmount > 9)
            {
                overheatImage.sprite = overheatImagesL0[int.Parse(overheatAmount.ToString()[0].ToString()) - 1];
            }
            else
            {
                overheatImage.sprite = overheatImagesL0[0];
            }
        }
        else if (gunLevel == 1)
        {
            Levels2And3Sprits(overheatImage, overheatImagesL1);
        }
        else if (gunLevel == 2)
        {
            Levels2And3Sprits(overheatImage, overheatImagesL2);
        }
    }

    private void Levels2And3Sprits(Image image, List<Sprite> imageList)
    {
        if (overheatAmount > 9 && overheatAmount < 100)
        {
            image.sprite = imageList[int.Parse(overheatAmount.ToString()[0].ToString()) - 1];
        }
        else if (overheatAmount > 99)
        {
            image.sprite = imageList[imageList.Count - 1];
        }
        else
        {
            image.sprite = imageList[0];
        }
    }

    public void EndGame()
    {
        gameOver = true;
        gameOverMenu.SetActive(true);
        gameOverMenu.GetComponentsInChildren<Transform>()[2].GetComponent<Text>().text = "Final money: " + String.Format("{0:n0}", money);
        DataBaseManager.money += money;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Kill(GameObject gameObject)
    {
        StartCoroutine(KillWithExplosion(gameObject));
    }

    private IEnumerator KillWithExplosion(GameObject gameObject)
    {
        if (gameObject.transform.position.x > -25)
        {
            int rotation = UnityEngine.Random.Range(0, 180);
            GameObject explosionGO = Instantiate(explosion, gameObject.transform.position, Quaternion.AngleAxis(rotation, Vector3.forward), gameObject.transform);
            explosionGO.GetComponent<Explosion>().parent = gameObject;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            if (gameObject.name != "Player")
            {
                gameObject.GetComponent<BaseEnemy>().canFireGuns = false;
                gameObject.GetComponent<BaseEnemy>().canFireTurrets = false;
            }
            yield return explosionGO.GetComponent<Explosion>().Die();
        }
        Destroy(gameObject);
    }

    public void CallUpdateDialogue()
    {
        StartCoroutine(UpdateDialogue());
    }

    private IEnumerator UpdateDialogue()
    {
        for (int i = 0; i < dialogues.Count; i++)
        {
            yield return new WaitForSeconds(timeBetweenDialogues[i]);
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
                if ((text.Length + 1) > 148)
                {
                    dialogueText.text = text.Substring(0, 148);
                    if (gameOver == false)
                    {
                        yield return new WaitForSeconds(5);
                        dialogueText.text = text.Substring(149);
                    }
                }
                else
                {
                    dialogueText.text = text;
                }
                yield return new WaitForSeconds(5);
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
}
