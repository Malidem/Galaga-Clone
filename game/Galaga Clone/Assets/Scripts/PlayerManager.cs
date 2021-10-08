using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public float maxSpeed = 500;
    private float speed;
    private float acceleration;
    private float deceleration;
    public float horizontalInput;
    public float verticalInput;
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

    private GameManager gameManager;
    private Rigidbody2D rigidb;
    private Vector2 position;
    private bool canFire = true;
    private bool canTakeDamage = true;
    private int healthAmount = 3;

    void Start()
    {
        gameManager = eventSystem.GetComponent<GameManager>();
        StartCoroutine(FireCooldown());
        rigidb = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        //horizontalInput = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
        //verticalInput = Input.GetAxis("Vertical") * Time.deltaTime * speed;

        if (gameManager.gameStarted && gameManager.gameOver == false)
        {
            //transform.Translate(Vector3.right * Time.deltaTime * horizontalInput * speed);
            //transform.Translate(Vector3.up * Time.deltaTime * verticalInput * speed);
            //rigidbody.AddForce(gameobject.transform.forward * (desired speed * accelerationFactor));
            //transform.Translate(horizontalInput, verticalInput, 0);
            //if (Input.GetKey(KeyCode.W))
            //{
            //    //rigidb.addforce(gameobject.transform.right * (speed * 2));
            //}
            //if (Input.GetKey(KeyCode.S))
            //{
            //    //rigidb.addforce(gameobject.transform.right * (speed * 2));
            //}
            //if (Input.GetKey(KeyCode.A))
            //{
            //    //rigidb.addforce(gameobject.transform.up * (speed * 2));
            //}
            //if (Input.GetKey(KeyCode.D))
            //{
            //    //rigidb.addforce(gameobject.transform.up * (speed * 2));
            //}

            if (Input.GetKey("left") && (speed < maxSpeed))
            {
                speed = speed - acceleration * Time.deltaTime;
            }
            else if (Input.GetKey("right") && (speed > - maxSpeed))
            {
                speed = speed + acceleration * Time.deltaTime;
            }
            else
            {
                if (speed > deceleration * Time.deltaTime)
                {
                    speed = speed - deceleration * Time.deltaTime;
                }
                else if (speed < -deceleration * Time.deltaTime)
                {
                    speed = speed + deceleration * Time.deltaTime;
                }
                else 
                {
                    speed = 0; 
                }    
            }
            position.x = transform.position.x + speed * Time.deltaTime;
            transform.position = position;

            if (Input.GetKey(KeyCode.Space))
            {
                if (canFire)
                {
                    Instantiate(bullet, transform.position, transform.rotation, bullets.transform);
                    canFire = false;
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
    }

    public IEnumerator RemoveHealth(int amount)
    {
        if (canTakeDamage)
        {
            canTakeDamage = false;
            for (int i = 0; i < amount; i++)
            {
                healthAmount -= 1;
            }

            UpdateHealthSprite();

            if (healthAmount > 0)
            {
                gameObject.GetComponent<Image>().color = Color.red;
                yield return new WaitForSeconds(0.2F);
                gameObject.GetComponent<Image>().color = Color.white;
                canTakeDamage = true;
            }
            else
            {
                gameManager.EndGame();
                Destroy(gameObject);
            }
        }
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
            gameManager.KillEnemy(collision.gameObject);
            StartCoroutine(RemoveHealth(1));
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
