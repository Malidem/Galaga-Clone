﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemy : BaseShip
{
    public int speed;
    public int moneyAwarded;
    public List<TurretsProps> turretsProps = new List<TurretsProps>();

    protected GameObject player;
    protected GameObject turretBulletFolder;
    protected int shieldTurretHealth = 2;
    protected float shieldRespawnInterval = 3;
    protected float shieldTurretSize = 1;
    protected float turretRotationSpeed = 100;

    private int currentShieldTurretHealth;
    private bool hasTurrets;
    private List<GameObject> turrets = new List<GameObject>();

    [HideInInspector]
    public bool canFireGuns = true;
    [HideInInspector]
    public bool canFireTurrets = true;
    [HideInInspector]
    public bool canShieldsTakeDamage = true;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        player = gameManager.player;
        turretBulletFolder = gameManager.turretBulletFolder;
        currentShieldTurretHealth = shieldTurretHealth;

        if (turretsProps.Count > 0)
        {
            hasTurrets = true;
        }

        if (hasTurrets)
        {
            bool hasShootingTurrets = false;
            for (int i = 0; i < turretsProps.Count; i++)
            {
                GameObject instance = Instantiate(turretsProps[i].turret, new Vector2(transform.position.x + turretsProps[i].position.x, transform.position.y + turretsProps[i].position.y), Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z + 180)), transform);
                turrets.Add(instance);
                if (!instance.name.Contains("Shield") && hasShootingTurrets == false)
                {
                    hasShootingTurrets = true;
                }
                else if (instance.name.Contains("Shield"))
                {
                    instance.transform.GetChild(0).transform.localScale = new Vector2(shieldTurretSize, shieldTurretSize);
                }
            }
            if (hasShootingTurrets)
            {
                StartCoroutine(FireTurretBullets());
            }
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (hasTurrets && gameManager.gameOver == false)
        {
            for (int i = 0; i < turrets.Count; i++)
            {
                RotateObjectToObject(turrets[i], player, turretRotationSpeed);
            }
        }
    }

    protected IEnumerator FireTurretBullets()
    {
        while (gameManager.gameOver == false && canFireTurrets)
        {
            yield return new WaitForSeconds(0.6F);
            if (IsOutOfBounds() == false)
            {
                int chance = UnityEngine.Random.Range(0, 101);
                if (chance <= 25)
                {
                    for (int i = 0; i < turretsProps.Count; i++)
                    {
                        if (!turrets[i].name.Contains("Shield"))
                        {
                            GameObject turret = turrets[i];
                            Instantiate(turretsProps[i].ammo, turret.transform.position, turret.transform.rotation, turretBulletFolder.transform);
                            audioSource.PlayOneShot(turretsProps[i].fireSound);
                        }
                    }
                }
            }
        }
    }

    public void RemoveShieldHealth(GameObject shield)
    {
        if (canShieldsTakeDamage)
        {
            currentShieldTurretHealth -= 1;
            if (currentShieldTurretHealth <= 0)
            {
                int i = turrets.IndexOf(shield.transform.parent.gameObject);
                Destroy(shield);
                StartCoroutine(RespawnShield(i));
            } 
        }
    }

    public IEnumerator RespawnShield(int index)
    {
        yield return new WaitForSeconds(shieldRespawnInterval);
        Transform instance = Instantiate(turretsProps[index].turret.transform.GetChild(0), turrets[index].transform);
        instance.localScale = new Vector2(shieldTurretSize, shieldTurretSize);
        currentShieldTurretHealth = shieldTurretHealth;
    }

    protected void Die()
    {
        gameManager.OnEnemyDestroyed(gameObject);
        Destroy(gameObject);
    }

    protected override void OnDeath()
    {
        gameManager.AddMoney(moneyAwarded);
    }

    protected bool IsOutOfBounds()
    {
        Rect rect = GetComponent<RectTransform>().rect;
        if ((transform.position.x - (rect.width / 2)) > backgroundRect.rect.width)
        {
            return true;
        }
        else if ((transform.position.y - (rect.height / 2)) > backgroundRect.rect.height)
        {
            return true;
        }
        else if ((transform.position.x + (rect.width / 2)) < 0)
        {
            return true;
        }
        else if ((transform.position.y + (rect.height / 2))  < 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    [Serializable]
    public class TurretsProps
    {
        public GameObject turret;
        public GameObject ammo;
        public AudioClip fireSound;
        public Vector2 position;
    }
}
