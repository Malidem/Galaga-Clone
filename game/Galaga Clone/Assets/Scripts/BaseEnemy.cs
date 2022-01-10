using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseEnemy : MonoBehaviour
{
    public int speed;
    public int health;
    public int moneyAwarded;

    public bool hasGuns;
    public GameObject bullets;
    public AudioClip gunFireSound;

    public bool hasTurret;
    public GameObject turretBullet;
    public GameObject turretType;
    public List<Vector2> turretPositions;
    public AudioClip turretFireSound;
    public int shieldTurretHealth;

    private int currentShieldTurretHealth;
    private bool isOrdaga4;
    private GameManager gameManager;
    private GameObject bulletFolder;
    private GameObject player;
    private GameObject background;
    private List<GameObject> turrets = new List<GameObject>();
    private RectTransform backgroundRect;
    private AudioSource audioSource;

    [HideInInspector]
    public bool canFireGuns = true;
    [HideInInspector]
    public bool canFireTurrets = true;

    // Start is called before the first frame update
    public void Start()
    {
        gameManager = GameObject.Find("EventSystem").GetComponent<GameManager>();
        bulletFolder = GameObject.Find("Bullets");
        player = gameManager.player;
        background = gameManager.background;
        backgroundRect = (RectTransform)background.transform;
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = PlayerPrefs.GetFloat(DataBaseManager.Prefs.soundVolume);
        currentShieldTurretHealth = shieldTurretHealth;

        if (health < 1)
        {
            Debug.LogError(gameObject.name + " health can not be less than 1");
        }

        if (hasGuns)
        {
            StartCoroutine(FireGunBullets());
        }

        if (hasTurret && turretPositions.Count > 0)
        {
            for (int i = 0; i < turretPositions.Count; i++)
            {
                GameObject instance = Instantiate(turretType, new Vector2(transform.position.x + turretPositions[i].x, transform.position.y + turretPositions[i].y), Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z + 180)), transform);
                turrets.Add(instance);
            }
            if (turretType.name != "Rakar4ShieldProjector")
            {
                StartCoroutine(FireTurretBullets());
            }
        }
        else if (hasTurret && turretPositions.Count <= 0)
        {
            Debug.LogError(gameObject.name + " has a turret, but not defind turret position");
        }

        if (gameObject.name.Contains("Ordaga4"))
        {
            isOrdaga4 = true;
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z + 180));
        }
        else
        {
            float backgroundCenter = backgroundRect.rect.height / 2;
            if (transform.position.y > (backgroundCenter + 175))
            {
                transform.rotation = Quaternion.Euler(0, 0, 25);
            }
            else if (transform.position.y < (backgroundCenter + -175))
            {
                transform.rotation = Quaternion.Euler(0, 0, -25);
            }
        }
    }

    public void Update()
    {
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

        if (transform.position.x > (backgroundRect.rect.width + 25))
        {
            Destroy(gameObject);
        }

        if (transform.position.y > (backgroundRect.rect.height + 25))
        {
            Destroy(gameObject);
        }

        if (transform.position.x < -25)
        {
            Destroy(gameObject);
        }

        if (transform.position.y < -25)
        {
            Destroy(gameObject);
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
                audioSource.PlayOneShot(gunFireSound);
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
                    audioSource.PlayOneShot(turretFireSound);
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
        yield return new WaitForSeconds(3);
        Instantiate(turretType.transform.GetChild(0), turrets[index].transform);
        currentShieldTurretHealth = shieldTurretHealth;
    }

    public void RemoveHealth()
    {
        health -= 1;
        if (health > 0)
        {
            StartCoroutine(FlashRed(gameObject));
            for (int i = 0; i < turrets.Count; i++)
            {
                StartCoroutine(FlashRed(turrets[i]));
            }
        }
        else
        {
            gameManager.Kill(gameObject, 1);
        }
    }

    private IEnumerator FlashRed(GameObject gameObject)
    {
        gameObject.GetComponent<Image>().color = Color.red;
        yield return new WaitForSeconds(0.2F);
        gameObject.GetComponent<Image>().color = Color.white;
    }
}
