using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullets : MonoBehaviour
{
    public int speed;
    private GameObject background;
    private GameObject player;
    private GameManager gameManager;

    void Start()
    {
        background =  GameObject.Find("Background");
        player = GameObject.Find("Player");
        gameManager = GameObject.Find("EventSystem").GetComponent<GameManager>();
    }

    void Update()
    {
        transform.Translate(Vector2.right * Time.deltaTime * speed);

        RectTransform rect = (RectTransform)background.transform;
        if (transform.position.x > rect.rect.width)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (gameManager.gameOver == false)
            {
                player.GetComponent<PlayerManager>().AddPoints(100);
            }
            gameManager.KillEnemy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
