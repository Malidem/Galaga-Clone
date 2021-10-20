using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullets : MonoBehaviour
{
    public int speed;
    public string dirction;
    private GameManager gameManager;
    private GameObject player;
    private GameObject background;

    void Start()
    {
        gameManager = GameObject.Find("EventSystem").GetComponent<GameManager>();
        player = gameManager.player;
        background = gameManager.background;
    }

    void Update()
    {
        if (dirction == "right")
        {
            transform.Translate(Vector2.right * Time.deltaTime * speed);
        }
        else
        {
            transform.Translate(Vector2.left * Time.deltaTime * speed);
        }

        RectTransform rect = (RectTransform)background.transform;
        if (transform.position.x > (rect.rect.width + 25))
        {
            Destroy(gameObject);
        }

        if (transform.position.y > (rect.rect.height + 25))
        {
            Destroy(gameObject);
        }

        if (transform.position.x < -25)
        {
            Destroy(gameObject);
        }

        if (transform.position.y < -25)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(player.GetComponent<PlayerManager>().RemoveHealth(1));
            StartCoroutine(Die());
        }
    }

    private IEnumerator Die()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, (transform.position.z - 100000));
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
