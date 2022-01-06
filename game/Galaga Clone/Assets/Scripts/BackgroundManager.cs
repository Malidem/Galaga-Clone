using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{
    public int speed;
    public GameObject backgroundPrefab;
    private GameObject eventSystem;
    private GameObject canvas;
    private GameManager gameManager;
    private bool canSpawn = true;

    // Start is called before the first frame update
    void Start()
    {
        eventSystem = GameObject.Find("EventSystem");
        gameManager = eventSystem.GetComponent<GameManager>();
        canvas = gameManager.canvas;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.gameStarted && gameManager.gameOver == false && gameManager.gamePaused == false)
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
                    GameObject bg = Instantiate(backgroundPrefab, new Vector2(transform.position.x + rect.rect.width, transform.position.y), rotation, canvas.GetComponentsInChildren<Transform>()[1]);
                    bg.name = backgroundPrefab.name;
                } 
            }
        }
    }
}
