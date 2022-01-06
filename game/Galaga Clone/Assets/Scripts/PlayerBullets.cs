using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullets : MonoBehaviour
{
    public int speed;
    private GameObject background;
    private GameManager gameManager;

    void Start()
    {
        background =  GameObject.Find("Backgrounds");
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (gameManager.gameOver == false)
            {
                gameManager.AddMoney(100);
            }
            gameManager.Kill(collision.gameObject, 1);
            Destroy(gameObject);
        }
    }
}
