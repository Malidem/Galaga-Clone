using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletManager : MonoBehaviour
{
    public int speed;
    private GameObject background;
    private GameObject player;

    void Start()
    {
        background = GameObject.Find("Background");
        player = GameObject.Find("Player");
    }

    void Update()
    {
        transform.Translate(Vector2.left * Time.deltaTime * speed);

        RectTransform rect = (RectTransform)background.transform;
        if (transform.position.x < 0)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player.GetComponent<PlayerManager>().RemoveHealth(1);
            Destroy(gameObject);
        }
    }
}
