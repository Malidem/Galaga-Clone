using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public int speed = 500;
    public float horizontalInput;
    public float verticalInput;
    public GameObject background;
    public GameObject bullet;
    public GameObject canvas;
    public GameObject eventSystem;
    public GameObject health;
    public GameObject bullets;
    
    private GameManager gameManager;
    private List<GameObject> allHealth = new List<GameObject>();
    private bool canFire = true;
    private bool canTakeDamage = true;

    void Start()
    {
        gameManager = eventSystem.GetComponent<GameManager>();
        AddHealth(3);
        StartCoroutine(FireCooldown());
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (gameManager.gameStarted && gameManager.gameOver == false)
        {
            transform.Translate(Vector3.right * Time.deltaTime * horizontalInput * speed);
            transform.Translate(Vector3.up * Time.deltaTime * verticalInput * speed);

            if (Input.GetKey(KeyCode.Space))
            {
                if (canFire)
                {
                    Instantiate(bullet, transform.position, transform.rotation, bullets.transform);
                    canFire = false;
                }
            }
        }

        RectTransform rect = (RectTransform)background.transform;
        RectTransform rect2 = (RectTransform)transform;
        float sizeX = rect2.rect.width / 2;
        float sizeY = rect2.rect.height / 2;

        if ((transform.position.x + sizeX) > (rect.rect.width / 2))
        {
            transform.position = new Vector2(((rect.rect.width / 2) - sizeX), transform.position.y);
        }

        if ((transform.position.y + sizeY) > rect.rect.height)
        {
            transform.position = new Vector2(transform.position.x, (rect.rect.height - sizeY));
        }

        if ((transform.position.x - sizeX) < 0)
        {
            transform.position = new Vector2((0 + sizeX), transform.position.y);
        }

        if ((transform.position.y - sizeY) < 0)
        {
            transform.position = new Vector2(transform.position.x, (0 + sizeY));
        }

        if (allHealth.Count <= 0)
        {
            gameManager.gameOver = true;
            Destroy(gameObject);
        }
    }

    public IEnumerator RemoveHealth(int amount)
    {
        // Damage cool down timer
        if (canTakeDamage)
        {
            for (int i = 0; i < amount; i++)
            {
                GameObject obj = allHealth[allHealth.Count - 1];
                allHealth.Remove(obj);
                Destroy(obj);
                canTakeDamage = false;
            }

            if (allHealth.Count > 0)
            {
                gameObject.GetComponent<Image>().color = Color.red;
                yield return new WaitForSeconds(0.2F);
                gameObject.GetComponent<Image>().color = Color.white;
                canTakeDamage = true; 
            }
        }
    }

    public void AddHealth(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            allHealth.Add(Instantiate(health, new Vector2(0, 0), gameManager.HUD.transform.rotation, gameManager.HUDElements[1]));
        }
    }

    public void AddPoints(int amount)
    {
        gameManager.points += amount;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            gameManager.KillEnemy(collision.gameObject);
            StartCoroutine(RemoveHealth(1));
        }
    }

    private IEnumerator FireCooldown()
    {
        while (gameManager.gameOver == false)
        {
            yield return new WaitForSeconds(0.25F);
            canFire = true;
        }
    }
}
