using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseShip : MonoBehaviour
{
    public int health;
    public bool canTakeDamage = true;

    protected int currentHealth;
    protected float explosionSize = 1;
    public GameManager gameManager;
    protected AudioSource audioSource;
    protected RectTransform backgroundRect;
    protected GameObject bulletFolder;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        gameManager = Resources.FindObjectsOfTypeAll<GameManager>()[0];
        backgroundRect = gameManager.backgroundRect;
        bulletFolder = gameManager.bulletFolder;
        audioSource = gameManager.audioSource;
        currentHealth = health;

        if (health < 1)
        {
            Debug.LogError(gameObject.name + " health can not be less than 1");
        }
    }

    public void AddHealth(int amount)
    {
        if ((currentHealth + amount) <= health)
        {
            currentHealth += amount;
        }
        OnHealthAdded();
    }

    protected virtual void OnHealthAdded() { }

    public virtual void RemoveHealth(int amount)
    {
        if (canTakeDamage && gameManager.gameOver == false)
        {
            canTakeDamage = false;
            currentHealth -= amount;

            OnHealthRemoved();

            if (currentHealth > 0)
            {
                StartCoroutine(FlashShip());
            }
            else
            {
                OnDeath();
                gameManager.Kill(gameObject, explosionSize);
            }
        }
    }

    protected virtual void OnHealthRemoved() { }

    protected virtual void OnDeath() { }

    private IEnumerator FlashShip()
    {
        if (CompareTag("Player"))
        {
            yield return StartCoroutine(FlashRed());
        }
        else
        {
            StartCoroutine(FlashRed());
        }
        canTakeDamage = true;
    }

    protected IEnumerator FlashRed()
    {
        ChangeShipColor(Color.red);
        yield return new WaitForSeconds(0.2F);
        ChangeShipColor(Color.white);
    }

    protected void ChangeShipColor(Color color)
    {
        gameObject.GetComponent<Image>().color = color;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (!child.CompareTag("OrdagaExplosionTrigger"))
            {
                child.GetComponent<Image>().color = color;
            }
        }
    }

    
    /// <summary>
    /// Incrimentaly rotates 'rotatable' towards the 'target' at the given 'speed'. Must be continisly updated.
    /// </summary>
    /// <param name="rotatable"></param>
    /// <param name="target"></param>
    /// <param name="speed"></param>
    protected void RotateObjectToObject(GameObject rotatable, GameObject target, float speed)
    {
        float angle = Mathf.Atan2(target.transform.position.y - rotatable.transform.position.y, target.transform.position.x - rotatable.transform.position.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        rotatable.transform.rotation = Quaternion.RotateTowards(rotatable.transform.rotation, targetRotation, speed * Time.deltaTime);
    }
}
