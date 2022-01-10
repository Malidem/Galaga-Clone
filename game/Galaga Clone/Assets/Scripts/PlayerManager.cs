using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public bool canTakeDamage = true;
    public GameObject background;
    public GameObject bullet;
    public GameObject canvas;
    public GameObject eventSystem;
    public GameObject healthBar;
    public GameObject bullets;
    public AudioClip gunFireSound;
    public List<Sprite> healthImagesL0 = new List<Sprite>();
    public List<Sprite> healthImagesL1 = new List<Sprite>();
    public List<Sprite> healthImagesL2 = new List<Sprite>();
    public List<GameObject> gunUpgrades = new List<GameObject>();
    public List<GameObject> healthUpgrades = new List<GameObject>();
    public List<GameObject> speedUpgrades = new List<GameObject>();

    private GameManager gameManager;
    private AudioSource audioSource;
    private bool canFire = true;
    private int healthAmount = 3;
    private float xSpeed = 0;
    private float ySpeed = 0;
    private float maxSpeed = 500;
    private float acceleration = 500;
    private float deceleration = 500;
    private int healthLevel = 0;

    void Start()
    {
        gameManager = eventSystem.GetComponent<GameManager>();
        StartCoroutine(FireCooldown());
        UpgradePlayer();
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = PlayerPrefs.GetFloat(DataBaseManager.Prefs.soundVolume);
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
                gameManager.Kill(gameObject, 1);
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
        Image healthImage = healthBar.GetComponent<Image>();
        if (healthLevel == 0)
        {
            healthImage.sprite = healthImagesL0[healthAmount + 1];
        }
        else if (healthLevel == 1)
        {
            healthImage.sprite = healthImagesL1[healthAmount + 1];
        }
        else if (healthLevel == 2)
        {
            healthImage.sprite = healthImagesL2[healthAmount + 1];
        }
    }

    private IEnumerator OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !collision.gameObject.name.Contains("Ordaga4"))
        {
            gameManager.Kill(collision.gameObject, 1);
            RemoveHealth(1);
        }
        else if (collision.gameObject.CompareTag("OrdagaExplosionTrigger"))
        {
            gameManager.Kill(collision.gameObject.transform.parent.gameObject, 1.7F);
            yield return new WaitForSeconds(0.4F);
            RemoveHealth(1);
        }
    }

    private IEnumerator FireCooldown()
    {
        while (gameManager.gameOver == false)
        {
            yield return new WaitForSeconds(0.25F);
            if (gameManager.overheatAmount <= (gameManager.overheatMax - 5) && gameManager.overheatCooldown == false)
            {
                canFire = true;
            }
            else
            {
                canFire = false;
            }
        }
    }

    private void UpgradePlayer()
    {
        for (int i = 0; i < DataBaseManager.upgradesActive.Length; i++)
        {
            string[] upgrade = DataBaseManager.upgradesActive[i].Split('|');
            if (upgrade[0] != "none")
            {
                int parsed = int.Parse(upgrade[1]);
                if (upgrade[0] == "gun")
                {
                    gunUpgrades[parsed - 1].SetActive(true);
                    gameManager.gunLevel = parsed;
                    gameManager.overheatMax += parsed * 50;
                }
                else if (upgrade[0] == "health")
                {
                    healthUpgrades[parsed - 1].SetActive(true);
                    healthLevel = parsed;
                    healthAmount += parsed;
                }
                else if (upgrade[0] == "speed")
                {
                    speedUpgrades[parsed - 1].SetActive(true);
                    maxSpeed += parsed * 50;
                    acceleration += parsed * 50;
                }
            }
        }

        UpdateHealthSprite();
        RectTransform HPBarRect = healthBar.GetComponent<Image>().rectTransform;
        int HPgained = 7 * healthLevel;
        HPBarRect.sizeDelta = new Vector2(32 + HPgained, HPBarRect.sizeDelta.y);
        HPBarRect.position = new Vector2(HPBarRect.position.x + HPgained, HPBarRect.position.y);

        if (gameManager.gunLevel > 0)
        {
            gameManager.UpdateOverheatSprite();
            RectTransform OHBarRect = gameManager.overheatBar.GetComponent<Image>().rectTransform;
            int OHgained = 10 * gameManager.gunLevel;
            OHBarRect.sizeDelta = new Vector2(OHBarRect.sizeDelta.x + OHgained, OHBarRect.sizeDelta.y);
            OHBarRect.position = new Vector2(OHBarRect.position.x + OHgained, OHBarRect.position.y);
        }
    }
}
