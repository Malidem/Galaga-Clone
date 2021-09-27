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
        canvas = GameObject.Find("Canvas");
        gameManager = eventSystem.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.gameStarted && gameManager.gameOver == false)
        {
            transform.Translate(Vector2.left * Time.deltaTime * speed);

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
                    GameObject bg = Instantiate(backgroundPrefab, new Vector2(transform.position.x + rect.rect.width, transform.position.y), transform.rotation, canvas.GetComponentsInChildren<Transform>()[1]);
                    bg.name = backgroundPrefab.name;
                } 
            }
        }
    }
}
