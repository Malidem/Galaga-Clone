using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RakarBoss : BaseEnemy
{
    public GameObject bullet;
    public AudioClip gunFireSound;
    public Vector2 rightBulletPosition;
    public Vector2 leftBulletPosition;

    private bool isMoving;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        FireGunBullets();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Start();
        if (true)
        {
            //transform.Translate(Vector2.up * Time.deltaTime * speed);
        }
        else
        {
            //transform.Translate(Vector2.down * Time.deltaTime * speed);
        }

        RectTransform bossRect = (RectTransform)transform;
        float sizeX = bossRect.rect.width / 2;
        float sizeY = bossRect.rect.height / 2;

        if ((transform.position.y + sizeY) > backgroundRect.rect.height)
        {
            transform.position = new Vector2(transform.position.x, backgroundRect.rect.height - sizeY);
            isMoving = false;
        }

        if ((transform.position.y - sizeY) < 0)
        {
            transform.position = new Vector2(transform.position.x, 0 - sizeY);
            isMoving = false;
        }
    }

    private IEnumerator FireGunBullets()
    {
        while (gameManager.gameOver == false && canFireGuns)
        {
            yield return new WaitForSeconds(0.5F);
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
