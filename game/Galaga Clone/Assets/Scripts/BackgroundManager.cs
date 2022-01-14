using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundManager : MonoBehaviour
{
    public int speed;
    public GameObject backgroundPrefab;
    private GameObject canvas;
    private GameObject backgroundsFolder;
    private GameManager gameManager;
    private MainMenuManager mainMenuManager;
    private bool canSpawn = true;
    private bool isOnMainMenu;

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Level")
        {
            gameManager = Resources.FindObjectsOfTypeAll<GameManager>()[0];
            canvas = gameManager.canvas;
            backgroundsFolder = gameManager.backgroundsFolder;
        }
        else
        {
            mainMenuManager = Resources.FindObjectsOfTypeAll<MainMenuManager>()[0];
            canvas = mainMenuManager.canvas;
            backgroundsFolder = mainMenuManager.backgroundsFolder;
            isOnMainMenu = true;
            speed = 75;
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool condition;

        if (isOnMainMenu)
        {
            condition = true;
        }
        else
        {
            condition = gameManager.gameStarted && gameManager.gameOver == false && gameManager.gamePaused == false;
        }

        if (condition)
        {
            if (transform.rotation.z != 0)
            {
                transform.Translate(Vector2.right * Time.deltaTime * speed);
            }
            else
            {
                transform.Translate(Vector2.left * Time.deltaTime * speed);
            }

            RectTransform rect = (RectTransform)transform;
            float edgeRPos = (transform.position.x + (rect.rect.width / 2));
            if (edgeRPos < 0)
            {
                Destroy(gameObject);
            }

            if (canSpawn)
            {
                if (edgeRPos < canvas.GetComponent<RectTransform>().rect.width)
                {
                    canSpawn = false;
                    Quaternion rotation;
                    if (Random.Range(1, 3) == 1)
                    {
                        rotation = Quaternion.Euler(0, 0, 180);
                    }
                    else
                    {
                        rotation = Quaternion.Euler(0, 0, 0);
                    }
                    GameObject bg = Instantiate(backgroundPrefab, new Vector2(transform.position.x + rect.rect.width, transform.position.y), rotation, backgroundsFolder.transform);
                    bg.name = backgroundPrefab.name;
                } 
            }
        }
    }
}
