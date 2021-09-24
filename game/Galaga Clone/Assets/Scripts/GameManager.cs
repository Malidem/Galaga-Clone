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
    public int OverheatAmount;
    public int points;

    [HideInInspector]
    public bool gameStarted;
    [HideInInspector]
    public List<GameObject> Enemies = new List<GameObject>();
    [HideInInspector]
    public bool gameOver;
    [HideInInspector]
    public Transform[] HUDElements;

    private int wave = 1;
    private int waveAmount = 2;

    void Start()
    {
        HUDElements = HUD.GetComponentsInChildren<Transform>();
        StartCoroutine(Overheat());
    }

    public void SpawnEnemies()
    {
        for (int i = 0; i < waveAmount; i++)
        {
            RectTransform rect = (RectTransform)background.transform;
            int num = UnityEngine.Random.Range(1, 4);
            if (num == 1)
            {
                Enemies.Add(Instantiate(enemyType1, new Vector2(rect.rect.width + 25, UnityEngine.Random.Range(0, rect.rect.height)), transform.rotation, enemies.transform));
            }
            else if (num == 2)
            {
                Enemies.Add(Instantiate(enemyType1, new Vector2(UnityEngine.Random.Range((rect.rect.width / 3) * 2, rect.rect.width + 25), rect.rect.height + 25), transform.rotation, enemies.transform));
            }
            else if (num == 3)
            {
                Enemies.Add(Instantiate(enemyType1, new Vector2(UnityEngine.Random.Range((rect.rect.width / 3) * 2, rect.rect.width + 25), -25), transform.rotation, enemies.transform));
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

            if (Enemies.Count <= 0)
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
            }

            HUDElements[2].gameObject.GetComponent<Text>().text = "Points: " + String.Format("{0:n0}", points);

            // each image is 12.5% less
            if (OverheatAmount >= 87)
            {
                overheatBar.sprite = overheatOverlay8;
            }
            else if (OverheatAmount >= 75 && OverheatAmount < 87)
            {
                overheatBar.sprite = overheatOverlay7;
            }
            else if (OverheatAmount >= 63 && OverheatAmount < 75)
            {
                overheatBar.sprite = overheatOverlay6;
            }
            else if (OverheatAmount >= 51 && OverheatAmount < 63)
            {
                overheatBar.sprite = overheatOverlay5;
            }
            else if (OverheatAmount >= 39 && OverheatAmount < 51)
            {
                overheatBar.sprite = overheatOverlay4;
            }
            else if (OverheatAmount >= 27 && OverheatAmount < 39)
            {
                overheatBar.sprite = overheatOverlay3;
            }
            else if (OverheatAmount >= 15 && OverheatAmount < 27)
            {
                overheatBar.sprite = overheatOverlay2;
            }
            else if (OverheatAmount >= 0 && OverheatAmount < 15)
            {
                overheatBar.sprite = overheatOverlay1;
            }
            else if (OverheatAmount >= 3 && OverheatAmount < 15)
            {
                overheatBar.sprite = overheatOverlay0;
            }
        }

        if (gameOver)
        {
            gameOverMenu.SetActive(true);
            gameOverMenu.GetComponentsInChildren<Transform>()[2].GetComponent<Text>().text = "Final Points: " + String.Format("{0:n0}", points);
        }
    }

    public void KillEnemy(GameObject enemy)
    {
        if (enemy != null)
        {
            Enemies.Remove(Enemies[Enemies.IndexOf(enemy)]);
            Destroy(enemy);
        }
    }

    private IEnumerator Overheat()
    {
        while (gameOver == false)
        {
            yield return new WaitForSeconds(0.2F);
            OverheatAmount -= 1;
        }
    }
}
