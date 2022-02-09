using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : BaseEnemy
{
    public bool hasGuns;
    public GameObject bullets;
    public AudioClip gunFireSound;

    private bool isOrdaga4;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        if (hasGuns)
        {
            StartCoroutine(FireGunBullets());
        }

        if (gameObject.name.Contains("Ordaga4"))
        {
            isOrdaga4 = true;
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z + 180));
        }
        else
        {
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
    }

    protected override void Update()
    {
        base.Update();
        if (isOrdaga4)
        {
            transform.Translate(Vector2.right * Time.deltaTime * speed);

            if (gameManager.gameOver == false)
            {
                float angle = Mathf.Atan2(player.transform.position.y - gameObject.transform.position.y, player.transform.position.x - gameObject.transform.position.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
                gameObject.transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, targetRotation, 10 * Time.deltaTime);
            }
        }
        else
        {
            transform.Translate(Vector2.left * Time.deltaTime * speed);
        }

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
