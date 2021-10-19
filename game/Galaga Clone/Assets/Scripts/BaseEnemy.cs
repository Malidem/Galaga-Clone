using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    public int speed;
    public GameObject hasGuns;
    public GameObject bullets;
    public GameObject hasTurret;
    public GameObject turretBullet;
    public List<Vector2> turretPositions;

    private GameManager gameManager;
    private GameObject canvas;
    private GameObject bulletFolder;

    [HideInInspector]
    public bool canFire = true;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("EventSystem").GetComponent<GameManager>();
        canvas = gameManager.canvas;
        bulletFolder = GameObject.Find("bullets");
        float y = canvas.transform.position.y;
        if (transform.position.y < (y - 25))
        {
            transform.rotation = Quaternion.Euler(0, 0, -25);
        }
        else if (transform.position.y > (y + 25))
        {
            transform.rotation = Quaternion.Euler(0, 0, 25);
        }
        StartCoroutine(SpawnBullets());
    }

    public IEnumerator SpawnBullets()
    {
        while (gameManager.gameOver == false && canFire)
        {
            yield return new WaitForSeconds(0.5F);
            int chance = Random.Range(1, 101);
            if (chance <= 25)
            {
                Instantiate(bullets, transform.position, transform.rotation, bulletFolder.transform);
            }
        }
    }

    void Update()
    {
        transform.Translate(Vector2.left * Time.deltaTime * speed);

        if (transform.position.x < -25)
        {
            gameManager.Kill(gameObject);
        }
    }
}
