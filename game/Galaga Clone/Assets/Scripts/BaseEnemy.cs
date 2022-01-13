using System;
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
    private int currentShieldTurretHealth;
    private bool hasTurrets;
    private List<GameObject> turrets = new List<GameObject>();

    [HideInInspector]
    public bool canFireGuns = true;

    [HideInInspector]
    public bool canFireTurrets = true;

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
                GameObject turret = turrets[i];
                float angle = Mathf.Atan2(player.transform.position.y - turret.transform.position.y, player.transform.position.x - turret.transform.position.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
                turret.transform.rotation = Quaternion.RotateTowards(turret.transform.rotation, targetRotation, 100 * Time.deltaTime);
            }
        }
    }

    private IEnumerator FireTurretBullets()
    {
        while (gameManager.gameOver == false && canFireTurrets)
        {
            yield return new WaitForSeconds(0.6F);
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

    public void RemoveShieldHealth(GameObject shield)
    {
        currentShieldTurretHealth -= 1;
        if (currentShieldTurretHealth <= 0)
        {
            int i = turrets.IndexOf(shield.transform.parent.gameObject);
            Destroy(shield);
            StartCoroutine(RespawnShield(i));
        }
    }

    public IEnumerator RespawnShield(int index)
    {
        yield return new WaitForSeconds(shieldRespawnInterval);
        Instantiate(turretsProps[index].turret.transform.GetChild(0), turrets[index].transform);
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

    [Serializable]
    public class TurretsProps
    {
        public GameObject turret;
        public GameObject ammo;
        public AudioClip fireSound;
        public Vector2 position;
    }
}
