﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    public int speed;
    public bool hasGuns;
    public GameObject bullets;
    public bool hasTurret;
    public GameObject turretBullet;
    public GameObject turretType;
    public List<Vector2> turretPositions;

    private GameManager gameManager;
    private GameObject canvas;
    private GameObject bulletFolder;
    private GameObject player;
    private List<GameObject> turrets = new List<GameObject>();

    [HideInInspector]
    public bool canFireGuns = true;
    [HideInInspector]
    public bool canFireTurrets = true;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("EventSystem").GetComponent<GameManager>();
        bulletFolder = GameObject.Find("Bullets");
        canvas = gameManager.canvas;
        player = gameManager.player;

        if (hasGuns)
        {
            StartCoroutine(FireGunBullets());
        }

        if (hasTurret && turretPositions.Count > 0)
        {
            for (int i = 0; i < turretPositions.Count; i++)
            {
                GameObject instance = Instantiate(turretType, new Vector2(transform.position.x + turretPositions[i].x, transform.position.y + turretPositions[i].y), transform.rotation, transform);
                turrets.Add(instance);
            }
            StartCoroutine(FireTurretBullets());
        }
        else if (hasTurret && turretPositions.Count <= 0)
        {
            Debug.LogError(gameObject.name + " has a turret, but not defind turret position");
        }

        float canvasCenter = canvas.transform.position.y / 2;
        if (transform.position.y < (canvasCenter - 75))
        {
            transform.rotation = Quaternion.Euler(0, 0, -25);
        }
        else if (transform.position.y > (canvasCenter + 75))
        {
            transform.rotation = Quaternion.Euler(0, 0, 25);
        }
    }

    void Update()
    {
        transform.Translate(Vector2.left * Time.deltaTime * speed);

        if (transform.position.x < -25)
        {
            gameManager.Kill(gameObject);
        }

        if (hasTurret && gameManager.gameOver == false)
        {
            for (int i = 0; i < turrets.Count; i++)
            {
                GameObject turret = turrets[i];
                float angle = Mathf.Atan2(player.transform.position.y - turret.transform.position.y, player.transform.position.x - turret.transform.position.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
                turret.transform.rotation = Quaternion.RotateTowards(turret.transform.rotation, targetRotation, 100 * Time.deltaTime);
            }
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
            }
        }
    }

    private IEnumerator FireTurretBullets()
    {
        while (gameManager.gameOver == false && canFireTurrets)
        {
            yield return new WaitForSeconds(0.6F);
            int chance = Random.Range(0, 101);
            if (chance <= 25)
            {
                for (int i = 0; i < turrets.Count; i++)
                {
                    GameObject turret = turrets[i];
                    Instantiate(turretBullet, turret.transform.position, turret.transform.rotation, bulletFolder.transform);
                }
            }
        }
    }
}
