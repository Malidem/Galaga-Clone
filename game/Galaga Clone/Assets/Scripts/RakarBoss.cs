using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RakarBoss : BaseBoss
{
    public GameObject backupEnemy;
    public GameObject bullet;
    public AudioClip gunFireSound;
    public Vector2 rightBulletPosition;
    public Vector2 leftBulletPosition;

    private bool isInPosition;
    private bool isMoving;
    private bool isTouchingTopEdge;
    private bool isTouchingBottomEdge;
    private bool canRunHalfHealthTask = true;
    private int direction;
    private float halfHeight;
    private RectTransform bossRect;
    private Vector2 destination;

    // Start is called before the first frame update
    protected override void Start()
    {
        shieldTurretSize = 0.5F;
        base.Start();
        shieldTurretHealth = 3;
        shieldRespawnInterval = 12;
        explosionSize = 3;
        bossRect = (RectTransform)transform;
        halfHeight = bossRect.rect.height / 2;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (isInPosition)
        {
            if (isMoving)
            {
                if (direction == 0)
                {
                    transform.Translate(Vector2.up * Time.deltaTime * speed);

                    if (transform.position.y > destination.y)
                    {
                        transform.position = new Vector2(transform.position.x, destination.y);
                        isMoving = false;
                    }
                }
                else
                {
                    transform.Translate(Vector2.down * Time.deltaTime * speed);

                    if (transform.position.y < destination.y)
                    {
                        transform.position = new Vector2(transform.position.x, destination.y);
                        isMoving = false;
                    }
                }
            }

            if ((transform.position.y + halfHeight) > backgroundRect.rect.height)
            {
                transform.position = new Vector2(transform.position.x, backgroundRect.rect.height - halfHeight);
                isMoving = false;
                isTouchingTopEdge = true;
            }
            else
            {
                isTouchingTopEdge = false;
            }

            if ((transform.position.y - halfHeight) < 0)
            {
                transform.position = new Vector2(transform.position.x, 0 + halfHeight);
                isMoving = false;
                isTouchingBottomEdge = true;
            }
            else
            {
                isTouchingBottomEdge = false;
            }

            if (currentHealth == (health / 2))
            {
                if (canRunHalfHealthTask)
                {
                    StartCoroutine(SpawnBackup());
                    canRunHalfHealthTask = false;
                }
            }
        }
        else
        {
            transform.Translate(Vector2.left * Time.deltaTime * speed);

            if (transform.position.x < (backgroundRect.rect.width - (((backgroundRect.rect.width / 2) / 2) / 2)))
            {
                isInPosition = true;
                canFireGuns = true;
                canFireTurrets = true;
                canTakeDamage = true;
                canShieldsTakeDamage = true;

                StartCoroutine(FireGunBullets());
                StartCoroutine(FireTurretBullets());
                StartCoroutine(DestinationController());
            }
        }
    }

    private IEnumerator DestinationController()
    {
        while (gameManager.gameOver == false)
        {
            if (isMoving == false)
            {
                yield return new WaitForSeconds(Random.Range(0, 4));
                if (isTouchingTopEdge)
                {
                    SetMoving(1);
                }
                else if (isTouchingBottomEdge)
                {
                    SetMoving(0);
                }
                else
                {
                    if (Random.Range(0, 2) == 0)
                    {
                        SetMoving(1);
                    }
                    else
                    {
                        SetMoving(0);
                    }
                }
            }
            else
            {
                yield return new WaitForSeconds(0.1F);
            }
        }
    }

    private void SetMoving(int direction)
    {
        float distance;
        if (direction == 0)
        {
            distance = Random.Range(backgroundRect.rect.height, transform.position.y + halfHeight);

        }
        else
        {
            distance = Random.Range(0, transform.position.y - halfHeight);
        }

        destination = new Vector2(transform.position.x, distance);
        isMoving = true;
        this.direction = direction;
    }

    private IEnumerator FireGunBullets()
    {
        while (gameManager.gameOver == false && canFireGuns)
        {
            yield return new WaitForSeconds(Random.Range(2, 5));
            CreateBullet(leftBulletPosition);
            CreateBullet(rightBulletPosition);
        }
    }

    private void CreateBullet(Vector2 position)
    {
        Instantiate(bullet, new Vector2(transform.position.x + position.x, transform.position.y + position.y), transform.rotation, bulletFolder.transform);
        audioSource.PlayOneShot(gunFireSound);
    }

    private IEnumerator SpawnBackup()
    {
        while (gameManager.gameOver == false)
        {
            for (int i = 0; i < 5; i++)
            {
                Transform spawnPoint = gameManager.enemySpawnPoints[Random.Range(0, gameManager.enemySpawnPoints.Count)];
                Instantiate(backupEnemy, spawnPoint.position, Quaternion.identity, gameManager.enemyFolder.transform);
            }
            yield return new WaitForSeconds(25);
        }
    }
}
