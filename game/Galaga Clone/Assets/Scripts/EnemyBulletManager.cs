using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletManager : MonoBehaviour
{
    public int speed;
    private GameObject background;
    private GameObject player;

    void Start()
    {
        background = GameObject.Find("Background");
        player = GameObject.Find("Player");
    }

    void Update()
    {
        transform.Translate(Vector2.left * Time.deltaTime * speed);

        RectTransform rect = (RectTransform)background.transform;
        if (transform.position.x < 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(player.GetComponent<PlayerManager>().RemoveHealth(1));
            StartCoroutine(DestroyThis());
        }
    }

    private IEnumerator DestroyThis()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, (transform.position.z - 100000));
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
