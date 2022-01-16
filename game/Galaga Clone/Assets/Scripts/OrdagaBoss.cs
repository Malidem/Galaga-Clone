using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrdagaBoss : BaseEnemy
{
    public int chargeSpeed;
    public GameObject teleportPoints;

    private bool isInPosition;
    private bool canCharge;
    private bool canTeleport;
    private List<Transform> teleportPointsList = new List<Transform>();
    private GameObject currentPoint;
    private Vector3 startPos;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        explosionSize = 2.5F;
        GameObject instance = Instantiate(teleportPoints, gameManager.enemyFolder.transform.position, Quaternion.identity, gameManager.enemyFolder.transform);
        instance.transform.SetSiblingIndex(2);
        for (int i = 0; i < instance.transform.childCount; i++)
        {
            teleportPointsList.Add(instance.transform.GetChild(i));
        }

        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z + 180));
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        for (int i = 0; i < teleportPointsList.Count; i++)
        {
            GameObject startGO = teleportPointsList[i].GetChild(0).gameObject;
            GameObject endGO = teleportPointsList[i].GetChild(1).gameObject;
            float angle = Mathf.Atan2(endGO.transform.position.y - startGO.transform.position.y, endGO.transform.position.x - startGO.transform.position.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            startGO.transform.rotation = Quaternion.RotateTowards(startGO.transform.rotation, targetRotation, 100 * Time.deltaTime);
        }
        if (isInPosition)
        {
            if (canTeleport)
            {
                StartCoroutine(TeleportRandomly());
            }

            if (gameManager.gameOver == false)
            {
                if (canCharge)
                {
                    transform.Translate(Vector2.right * Time.deltaTime * chargeSpeed);
                }
            }
        }
        else
        {
            transform.Translate(Vector2.right * Time.deltaTime * speed);

            if (transform.position.x < (backgroundRect.rect.width - ((backgroundRect.rect.width / 2) / 2)))
            {
                StartCoroutine(OnceInPosition());
            }
        }
    }

    private IEnumerator OnceInPosition()
    {
        isInPosition = true;
        canFireGuns = true;
        canFireTurrets = true;
        canTakeDamage = true;
        canShieldsTakeDamage = true;

        StartCoroutine(FireTurretBullets());

        yield return new WaitForSeconds(2);

        canCharge = true;
    }

    private IEnumerator TeleportRandomly()
    {
        canTeleport = false;
        currentPoint = teleportPointsList[Random.Range(0, teleportPointsList.Count)].gameObject;
        GameObject startGO = currentPoint.transform.GetChild(0).gameObject;
        startPos = startGO.transform.position;
        yield return new WaitForSeconds(2);
        transform.position = new Vector2(startPos.x, startPos.y);
        transform.rotation = startGO.transform.rotation;
        canCharge = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "End")
        {
            canCharge = false;
            canTeleport = true;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerManager>().RemoveHealth(3);
        }
    }
}
