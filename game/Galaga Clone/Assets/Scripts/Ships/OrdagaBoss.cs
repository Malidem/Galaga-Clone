using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrdagaBoss : BaseBoss
{
    public int chargeSpeed;
    public GameObject teleportPoints;

    private int lastPointSet;
    private bool isInPosition;
    private bool canCharge;
    private bool canTeleport;
    private bool canInflictCollsionDamage = true;
    private List<Transform> teleportPointsList = new List<Transform>();
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
            GameObject point1 = teleportPointsList[i].GetChild(0).gameObject;
            GameObject point2 = teleportPointsList[i].GetChild(1).gameObject;

            float point1Angle = Mathf.Atan2(point2.transform.position.y - point1.transform.position.y, point2.transform.position.x - point1.transform.position.x) * Mathf.Rad2Deg;
            Quaternion point1TargetRotation = Quaternion.AngleAxis(point1Angle, Vector3.forward);
            point1.transform.rotation = Quaternion.RotateTowards(point1.transform.rotation, point1TargetRotation, 100 * Time.deltaTime);

            float point2Angle = Mathf.Atan2(point1.transform.position.y - point2.transform.position.y, point1.transform.position.x - point2.transform.position.x) * Mathf.Rad2Deg;
            Quaternion point2TargetRotation = Quaternion.AngleAxis(point2Angle, Vector3.forward);
            point2.transform.rotation = Quaternion.RotateTowards(point2.transform.rotation, point2TargetRotation, 100 * Time.deltaTime);
        }

        if (isInPosition)
        {
            if (canTeleport)
            {
                StartCoroutine(TeleportRandomly());
            }

            if (canCharge)
            {
                transform.Translate(Vector2.right * Time.deltaTime * chargeSpeed);
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

        if (IsOutOfBounds())
        {
            turretRotationSpeed = 300;
        }
        else
        {
            turretRotationSpeed = 100;
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
        if (gameManager.gameOver == false)
        {
            canTeleport = false;
            GameObject currentPointSet;

            choosePointSet:
            int nextPointSet = Random.Range(0, teleportPointsList.Count);
            if (nextPointSet != lastPointSet)
            {
                currentPointSet = teleportPointsList[nextPointSet].gameObject;
                lastPointSet = nextPointSet;
            }
            else
            {
                goto choosePointSet;
            }

            GameObject startGO = currentPointSet.transform.GetChild(Random.Range(0, 2)).gameObject;
            startPos = startGO.transform.position;
            yield return new WaitForSeconds(2);
            StartCoroutine(TemporarilyTurnOffCollider(startGO.GetComponent<BoxCollider2D>()));
            transform.SetPositionAndRotation(new Vector2(startPos.x, startPos.y), startGO.transform.rotation);
            canCharge = true;
        }
    }

    private IEnumerator TemporarilyTurnOffCollider(BoxCollider2D collider2D)
    {
        collider2D.enabled = false;
        yield return new WaitForSeconds(1);
        collider2D.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Contains("Point"))
        {
            canCharge = false;
            canTeleport = true;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            canFireTurrets = false;
            if (canInflictCollsionDamage)
            {
                canInflictCollsionDamage = false;
                collision.gameObject.GetComponent<PlayerManager>().RemoveHealth(3);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            canInflictCollsionDamage = true;
            canFireTurrets = true;
            StartCoroutine(FireTurretBullets());
        }
    }
}
