using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ordaga4Manager : BaseEnemy
{
    public AudioClip detonationSound;
    public Sprite[] sprites;

    private float baseSize;
    private bool canDetontate = true;
    private Transform detonation;
    private RectTransform detonationRect;
    private BoxCollider2D detonationCollider;

    // Start is called before the first frame update
    protected override void Start()
    {
        detonation = transform.GetChild(1);
        detonationRect = detonation.GetComponent<RectTransform>();
        detonationCollider = detonation.GetComponent<BoxCollider2D>();
        baseSize = detonationRect.sizeDelta.x / sprites.Length;
        base.Start();
        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z + 180));
    }

    // Update is called once per frame
    protected override void Update()
    {
        transform.Translate(Vector2.right * Time.deltaTime * speed);

        if (gameManager.gameOver == false)
        {
            float angle = Mathf.Atan2(player.transform.position.y - gameObject.transform.position.y, player.transform.position.x - gameObject.transform.position.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            gameObject.transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, targetRotation, 10 * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && canDetontate)
        {
            canDetontate = false;
            GetComponent<BoxCollider2D>().enabled = false;
            GetComponent<Image>().enabled = false;
            StartCoroutine(Detonate());
        }
    }

    public IEnumerator Detonate()
    {
        float time = 2 / sprites.Length;
        GetComponent<AudioSource>().PlayOneShot(detonationSound);
        detonation.gameObject.SetActive(true);
        for (int i = 0; i < sprites.Length; i++)
        {
            Vector2 size = new Vector2(baseSize * i, baseSize * i);
            detonationRect.sizeDelta = size;
            detonationCollider.size = size;
            detonation.GetComponent<Image>().sprite = sprites[i];
            yield return new WaitForSeconds(time);
        }
        Die();
    }
}
