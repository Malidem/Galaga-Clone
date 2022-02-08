﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : BaseShip
{
    public GameObject bullet;
    public GameObject healthBar;
    public AudioClip gunFireSound;
    public List<Sprite> healthImagesL0 = new List<Sprite>();
    public List<Sprite> healthImagesL1 = new List<Sprite>();
    public List<Sprite> healthImagesL2 = new List<Sprite>();
    public List<GameObject> gunUpgrades = new List<GameObject>();
    public List<GameObject> speedUpgrades = new List<GameObject>();

    private bool canFire = true;
    private float xSpeed = 0;
    private float ySpeed = 0;
    private float maxSpeed = 500;
    private float acceleration = 500;
    private float deceleration = 500;
    private int healthLevel = 0;
    private int regenerationLevel = 0;
    private int[] regenerationTime = { 15, 10, 5 };

    protected override void Start()
    {
        base.Start();
        StartCoroutine(FireCooldown());
        UpgradePlayer();
        StartCoroutine(RegenerateHealth());
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
                    Instantiate(bullet, transform.position, transform.rotation, bulletFolder.transform);
                    canFire = false;
                    audioSource.PlayOneShot(gunFireSound);
                    gameManager.overheatAmount += 5;
                    gameManager.UpdateOverheatSprite();
                }
            }
        }

        RectTransform playerRect = (RectTransform)transform;
        float sizeX = playerRect.rect.width / 2;
        float sizeY = playerRect.rect.height / 2;

        if ((transform.position.x + sizeX) > (backgroundRect.rect.width / 2))
        {
            transform.position = new Vector2(((backgroundRect.rect.width / 2) - sizeX), transform.position.y);
            xSpeed = 0;
        }

        if ((transform.position.y + sizeY) > backgroundRect.rect.height)
        {
            transform.position = new Vector2(transform.position.x, (backgroundRect.rect.height - sizeY));
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

    protected override void OnHealthAdded()
    {
        UpdateHealthSprite();
    }

    protected override void OnHealthRemoved()
    {
        UpdateHealthSprite();
    }

    protected override void OnDeath()
    {
        gameManager.EndGame();
    }

    private void UpdateHealthSprite()
    {
        if (currentHealth >= 0)
        {
            Image healthImage = healthBar.GetComponent<Image>();
            if (healthLevel == 0)
            {
                healthImage.sprite = healthImagesL0[currentHealth];
            }
            else if (healthLevel == 1)
            {
                healthImage.sprite = healthImagesL1[currentHealth];
            }
            else if (healthLevel == 2)
            {
                healthImage.sprite = healthImagesL2[currentHealth];
            }
        }
    }

    private IEnumerator OnTriggerEnter2D(Collider2D collision)
    {
        GameObject gObject = collision.gameObject;
        if (!gObject.CompareTag("EnemyBullet") && !gObject.name.Contains("OrdagaBoss"))
        {
            if (gObject.CompareTag("Enemy") && !gObject.name.Contains("Ordaga4") )
            {
                gameManager.Kill(gObject, 1);
            }
            else if (gObject.CompareTag("OrdagaExplosionTrigger"))
            {
                gameManager.Kill(gObject.transform.parent.gameObject, 1.7F);
                yield return new WaitForSeconds(0.4F);
            }
            else if (gObject.CompareTag("BossEnemy"))
            {
                gObject.GetComponent<BaseEnemy>().RemoveHealth(1);
            }
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
                    gunUpgrades[0].SetActive(false);
                    gunUpgrades[parsed].SetActive(true);
                    gameManager.gunLevel = parsed;
                    gameManager.overheatMax += parsed * 50;
                }
                else if (upgrade[0] == "health")
                {
                    healthLevel = parsed;
                    health += parsed;
                    currentHealth = health;
                }
                else if (upgrade[0] == "speed")
                {
                    speedUpgrades[0].SetActive(false);
                    speedUpgrades[parsed].SetActive(true);
                    maxSpeed += parsed * 50;
                    acceleration += parsed * 50;
                }
                else if (upgrade[0] == "regeneration")
                {
                    regenerationLevel = parsed;
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

    private IEnumerator RegenerateHealth()
    {
        if (regenerationLevel > 0)
        {
            while (gameManager.gameOver == false)
            {
                yield return new WaitForSeconds(regenerationTime[regenerationLevel - 1]);
                if (currentHealth < health)
                {
                    AddHealth(1);
                }
            }
        }
    }
}
