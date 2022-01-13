﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RakarBoss : BaseEnemy
{
    public GameObject bullet;
    public AudioClip gunFireSound;
    public Vector2 rightBulletPosition;
    public Vector2 leftBulletPosition;

    private bool isMoving;
    private bool isTouchingTopEdge;
    private bool isTouchingBottomEdge;
    private int direction;
    private float halfHeight;
    private RectTransform bossRect;
    private Vector2 destination;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        shieldTurretHealth = 5;
        shieldRespawnInterval = 6;
        explosionSize = 3;
        bossRect = (RectTransform)transform;
        halfHeight = bossRect.rect.height / 2;
        StartCoroutine(FireGunBullets());
        StartCoroutine(DestinationPicker());
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
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
    }

    private IEnumerator DestinationPicker()
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
            yield return new WaitForSeconds(1);
            int chance = Random.Range(0, 101);
            if (chance <= 25)
            {
                int actionInt = Random.Range(0, 3);
                if (actionInt == 0)
                {
                    CreateBullet(leftBulletPosition);
                }
                else if (actionInt == 1)
                {
                    CreateBullet(rightBulletPosition);
                }
                else
                {
                    yield return new WaitForSeconds(1);
                    CreateBullet(leftBulletPosition);
                    CreateBullet(rightBulletPosition);
                }
            }
        }
    }

    private void CreateBullet(Vector2 position)
    {
        Instantiate(bullet, new Vector2(transform.position.x + position.x, transform.position.y + position.y), transform.rotation, bulletFolder.transform);
        audioSource.PlayOneShot(gunFireSound);
    }
}
