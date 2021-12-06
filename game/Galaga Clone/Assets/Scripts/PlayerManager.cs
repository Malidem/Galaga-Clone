using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public GameObject background;
    public GameObject bullet;
    public GameObject canvas;
    public GameObject eventSystem;
    public GameObject healthBar;
    public GameObject bullets;
    public Sprite healthImage0;
    public Sprite healthImage1;
    public Sprite healthImage2;
    public Sprite healthImage3;
    public AudioClip gunFireSound;

    private GameManager gameManager;
    private AudioSource audioSource;
    private bool canFire = true;
    private bool canTakeDamage = true;
    private int healthAmount = 3;
    private float xSpeed = 0;
    private float ySpeed = 0;
    private float maxSpeed = 500;
    private float acceleration = 550;
    private float deceleration = 500;

    void Start()
    {
        gameManager = eventSystem.GetComponent<GameManager>();
        StartCoroutine(FireCooldown());
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {

        if (gameManager.gameStarted && gameManager.gameOver == false && gameManager.gamePaused == false)
        {

            if ((Input.GetKey(KeyCode.A)) && (xSpeed < maxSpeed))
            {
                xSpeed = xSpeed - acceleration * Time.deltaTime;
            }
            else if ((Input.GetKey(KeyCode.D)) && (xSpeed > -maxSpeed))
            {
                xSpeed = xSpeed + acceleration * Time.deltaTime;
            }
            else
            {
                if (xSpeed > deceleration * Time.deltaTime)
                {
                    xSpeed = xSpeed - deceleration * Time.deltaTime;
                }
                else if (xSpeed < -deceleration * Time.deltaTime)
                {
                    xSpeed = xSpeed + deceleration * Time.deltaTime;
                }
                else
                {
                    xSpeed = 0;
                }
            }

            if ((Input.GetKey(KeyCode.S)) && (ySpeed < maxSpeed))
            {
                ySpeed = ySpeed - acceleration * Time.deltaTime;
            }
            else if ((Input.GetKey(KeyCode.W)) && (ySpeed > -maxSpeed))
            {
                ySpeed = ySpeed + acceleration * Time.deltaTime;
            }
            else
            {
                if (ySpeed > deceleration * Time.deltaTime)
                {
                    ySpeed = ySpeed - deceleration * Time.deltaTime;
                }
                else if (ySpeed < -deceleration * Time.deltaTime)
                {
                    ySpeed = ySpeed + deceleration * Time.deltaTime;
                }
                else
                {
                    ySpeed = 0;
                }
            }
            transform.position = new Vector2(transform.position.x + xSpeed * Time.deltaTime, transform.position.y + ySpeed * Time.deltaTime);

            if (Input.GetKey(KeyCode.Space))
            {
                if (canFire)
                {
                    Instantiate(bullet, transform.position, transform.rotation, bullets.transform);
                    canFire = false;
                    audioSource.PlayOneShot(gunFireSound);
                    gameManager.overheatAmount += 5;
                    gameManager.UpdateOverheatSprite();
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
            xSpeed = 0;
        }

        if ((transform.position.y + sizeY) > rect.rect.height)
        {
            transform.position = new Vector2(transform.position.x, (rect.rect.height - sizeY));
            ySpeed = 0;
        }

        if ((transform.position.x - sizeX) < 0)
        {
            transform.position = new Vector2((0 + sizeX), transform.position.y);
            xSpeed = 0;
        }

        if ((transform.position.y - sizeY) < 0)
        {
            transform.position = new Vector2(transform.position.x, (0 + sizeY));
            ySpeed = 0;
        }
    }

    public void RemoveHealth(int amount)
    {
        if (canTakeDamage)
        {
            canTakeDamage = false;
            for (int i = 0; i < amount; i++)
            {
                healthAmount -= 1;
            }

            UpdateHealthSprite();

            if (healthAmount > -1)
            {
                StartCoroutine(FlashRed());
                canTakeDamage = true;
            }
            else
            {
                gameManager.EndGame();
                gameManager.Kill(gameObject);
            }
        }
    }

    private IEnumerator FlashRed()
    {
        gameObject.GetComponent<Image>().color = Color.red;
        yield return new WaitForSeconds(0.2F);
        gameObject.GetComponent<Image>().color = Color.white;
    }

    public void AddHealth(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            healthAmount += 1;
        }
        UpdateHealthSprite();
    }

    private void UpdateHealthSprite()
    {
        if (healthAmount == 3)
        {
            healthBar.GetComponent<Image>().sprite = healthImage0;
        }
        else if (healthAmount == 2)
        {
            healthBar.GetComponent<Image>().sprite = healthImage1;
        }
        else if (healthAmount == 1)
        {
            healthBar.GetComponent<Image>().sprite = healthImage2;
        }
        else if (healthAmount == 0)
        {
            healthBar.GetComponent<Image>().sprite = healthImage3;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            gameManager.Kill(collision.gameObject);
            RemoveHealth(1);
        }
    }

    private IEnumerator FireCooldown()
    {
        while (gameManager.gameOver == false)
        {
            yield return new WaitForSeconds(0.25F);
            if (gameManager.overheatAmount <= 95 && gameManager.overheatCooldown == false)
            {
                canFire = true;
            }
            else
            {
                canFire = false;
            }
        }
    }
}
