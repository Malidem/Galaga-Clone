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
}
