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
    [TextArea(3, 10)] [SerializeField] protected List<string> dialoges = new List<string>();
    public List<float> timeBetweenDialogues = new List<float>();
    public Texture2D cursor;
    public int overheatAmount;
    public int points;
    public List<GameObject> enemyTypes;
    
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

    private int wave;
    private int waveAmount = 1;

    void Start()
    {
        Time.timeScale = 1;
        HUDElements = HUD.GetComponentsInChildren<Transform>();
        StartCoroutine(Overheat());
        Instantiate(backgroundPrefab, background.transform.position, transform.rotation, canvas.GetComponentsInChildren<Transform>()[1]);
        Cursor.SetCursor(cursor, Vector3.zero, CursorMode.ForceSoftware);

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

    public void AddPoints(int amount)
    {
        points += amount;
        HUDElements[2].gameObject.GetComponent<Text>().text = "Points: " + String.Format("{0:n0}", points);
    }

    private IEnumerator Overheat()
    {
        while (gameOver == false)
        {
            yield return new WaitForSeconds(0.15F);
            if (overheatAmount > 95)
            {
                overheatCooldown = true;
            }
            if (overheatAmount > 0)
            {
                if (overheatCooldown)
                {
                    overheatAmount -= 5;
                    UpdateOverheatSprite();
                    if (overheatAmount <= 0)
                    {
                        overheatCooldown = false;
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
        // each image is 12.5% less
        if (overheatAmount <= 6.25F)
        {
            overheatBar.sprite = overheatOverlay0;
        }
        else if (overheatAmount <= 12.5F && overheatAmount > 6.25F)
        {
            overheatBar.sprite = overheatOverlay1;
        }
        else if (overheatAmount <= 25 && overheatAmount > 12.5F)
        {
            overheatBar.sprite = overheatOverlay2;
        }
        else if (overheatAmount <= 37.5F && overheatAmount > 25)
        {
            overheatBar.sprite = overheatOverlay3;
        }
        else if (overheatAmount <= 50 && overheatAmount > 37.5F)
        {
            overheatBar.sprite = overheatOverlay4;
        }
        else if (overheatAmount <= 62.5F && overheatAmount > 50)
        {
            overheatBar.sprite = overheatOverlay5;
        }
        else if (overheatAmount <= 75 && overheatAmount > 62.5F)
        {
            overheatBar.sprite = overheatOverlay6;
        }
        else if (overheatAmount <= 87.5F && overheatAmount > 75)
        {
            overheatBar.sprite = overheatOverlay7;
        }
        else if (overheatAmount <= 100 && overheatAmount > 87.5F)
        {
            overheatBar.sprite = overheatOverlay8;
        }
    }

    public void EndGame()
    {
        gameOver = true;
        gameOverMenu.SetActive(true);
        gameOverMenu.GetComponentsInChildren<Transform>()[2].GetComponent<Text>().text = "Final Points: " + String.Format("{0:n0}", points);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Kill(GameObject gameObject, float scale)
    {
        StartCoroutine(KillWithExplosion(gameObject, scale)) ;
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
        for (int i = 0; i < dialoges.Count; i++)
        {
            string text = dialoges[i];
            yield return new WaitForSeconds(timeBetweenDialogues[i]);
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
            if ((text.Length + 1) > 148) {
                dialogueText.text = text.Substring(0, 148);
                yield return new WaitForSeconds(5);
                dialogueText.text = text.Substring(149);
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
    }
}
