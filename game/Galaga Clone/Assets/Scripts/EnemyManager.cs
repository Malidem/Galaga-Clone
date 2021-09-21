using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject canvas;
    public GameObject bullet;
    private GameObject bullets;
    private GameManager gameManager;
    private float rotate = 25;
    private float speed = 100;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.Find("Canvas");
        bullets = GameObject.Find("Bullets");
        gameManager = GameObject.Find("EventSystem").GetComponent<GameManager>();
        float y = canvas.transform.position.y;
        if (transform.position.y < y)
        {
            transform.rotation = Quaternion.Euler(0, 0, -rotate);
        }
        if (transform.position.y > y) 
        {
            transform.rotation = Quaternion.Euler(0, 0, rotate);
        }
        StartCoroutine(SpawnBullets());
    }

    public IEnumerator SpawnBullets()
    {
        yield return new WaitForSeconds(0.5F);
        Instantiate(bullet, transform.position, transform.rotation, bullets.transform);
    }

    void Update()
    {
        //Vector2 direction = player.transform.position - transform.position;
        //float angle = Vector2.Angle(direction, transform.forward);
        //print("angle: " + angle);
        transform.Translate(Vector2.left * Time.deltaTime * speed);

        if (transform.position.x < -25)
        {
            gameManager.KillEnemy(gameObject);
        }
    }
}
