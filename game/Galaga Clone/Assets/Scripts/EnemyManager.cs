using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : BaseEnemy
{
    public bool hasGuns;
    public GameObject bullets;
    public AudioClip gunFireSound;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        if (hasGuns)
        {
            StartCoroutine(FireGunBullets());
        }
        float backgroundCenter = backgroundRect.rect.height / 2;
        if (transform.position.y > (backgroundCenter + 201))
        {
            transform.rotation = Quaternion.Euler(0, 0, 25);
        }
        else if (transform.position.y < (backgroundCenter + -201))
        {
            transform.rotation = Quaternion.Euler(0, 0, -25);
        }
    }

    protected override void Update()
    {
        base.Update();
        transform.Translate(Vector2.left * Time.deltaTime * speed);

        if (transform.position.x > (backgroundRect.rect.width + 100))
        {
            Die();
        }

        if (transform.position.y > (backgroundRect.rect.height + 55))
        {
            Die();
        }

        if (transform.position.x < -25)
        {
            Die();
        }

        if (transform.position.y < -55)
        {
            Die();
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
                Instantiate(bullets, transform.position, transform.rotation, bulletFolder.transform);
                audioSource.PlayOneShot(gunFireSound);
            }
        }
    }
}
