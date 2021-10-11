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
    public int overheatAmount;
    public int points;

    [HideInInspector]
    public bool gameStarted;
    [HideInInspector]
    public bool gameOver;
    [HideInInspector]
    public Transform[] HUDElements;
    [HideInInspector]
    public bool overheatCooldown;

    private int wave;
    private int waveAmount = 1;

    void Start()
    {
        Time.timeScale = 1;
        HUDElements = HUD.GetComponentsInChildren<Transform>();
        StartCoroutine(Overheat());
        Instantiate(backgroundPrefab, background.transform.position, transform.rotation, canvas.GetComponentsInChildren<Transform>()[1]);
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
            int num = UnityEngine.Random.Range(1, 4);
            if (num == 1)
            {
                Instantiate(enemyType1, new Vector2(rect.rect.width + 25, UnityEngine.Random.Range(0, rect.rect.height)), transform.rotation, enemies.transform);
            }
            else if (num == 2)
            {
                Instantiate(enemyType1, new Vector2(UnityEngine.Random.Range((rect.rect.width / 3) * 2, rect.rect.width + 25), rect.rect.height + 25), transform.rotation, enemies.transform);
            }
            else if (num == 3)
            {
                Instantiate(enemyType1, new Vector2(UnityEngine.Random.Range((rect.rect.width / 3) * 2, rect.rect.width + 25), -25), transform.rotation, enemies.transform);
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
                }
                else
                {
                    Time.timeScale = 0;
                    pauseMenu.SetActive(true);
                }
            }
        }
    }

    public void AddPoints(int amount)
    {
        points += amount;
        HUDElements[2].gameObject.GetComponent<Text>().text = "Points: " + String.Format("{0:n0}", points);
    }

    public void KillEnemy(GameObject enemy)
    {
        if (enemy != null)
        {
            Destroy(enemy);
        }
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
    }
}
