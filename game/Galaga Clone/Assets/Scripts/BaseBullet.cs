using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBullet : MonoBehaviour
{
    public int speed;
    public int damageAmount = 1;
    public bool isEnemyBullet;
    public Directions dirction;

    private GameManager gameManager;
    private GameObject player;
    private RectTransform backgroundRect;
    private RectTransform bulletRect;

    void Start()
    {
        bulletRect = (RectTransform)transform;
        gameManager = Resources.FindObjectsOfTypeAll<GameManager>()[0];
        player = gameManager.player;
        backgroundRect = gameManager.backgroundRect;
    }

    void Update()
    {
        if (dirction == Directions.Right)
        {
            transform.Translate(Vector2.right * Time.deltaTime * speed);
        }
        else
        {
            transform.Translate(Vector2.left * Time.deltaTime * speed);
        }

        if (transform.position.x > (backgroundRect.rect.width + bulletRect.rect.width))
        {
            Destroy(gameObject);
        }

        if (transform.position.y > (backgroundRect.rect.height + bulletRect.rect.width))
        {
            Destroy(gameObject);
        }

        if (transform.position.x < -bulletRect.rect.width)
        {
            Destroy(gameObject);
        }

        if (transform.position.y < -bulletRect.rect.width)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject gObject = collision.gameObject;
        if (isEnemyBullet)
        {
            if (gObject.CompareTag("Player"))
            {
                gObject.GetComponent<PlayerManager>().RemoveHealth(damageAmount);
            }
        }
        else
        {
            if (gObject.CompareTag("Enemy") || gObject.CompareTag("BossEnemy"))
            {
                gObject.GetComponent<BaseEnemy>().RemoveHealth(1);
            }
            else if (gObject.CompareTag("EnemyShield"))
            {
                gObject.transform.parent.parent.GetComponent<BaseEnemy>().RemoveShieldHealth(gObject);
            }
        }

        if (!gObject.CompareTag("OrdagaExplosionTrigger"))
        {
            Destroy(gameObject);
        }
    }

    [Serializable]
    public enum Directions
    {
        Left,
        Right,
    }
}
