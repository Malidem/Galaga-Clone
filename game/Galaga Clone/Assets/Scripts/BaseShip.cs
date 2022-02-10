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
    protected GameManager gameManager;
    protected AudioSource audioSource;
    protected RectTransform backgroundRect;
    protected GameObject bulletFolder;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        gameManager = Resources.FindObjectsOfTypeAll<GameManager>()[0];
        backgroundRect = gameManager.backgroundRect;
        bulletFolder = gameManager.bulletFolder;
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = PlayerPrefs.GetFloat(DataBaseManager.Prefs.soundVolume);
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
                StartCoroutine(FlashRed(gameObject));
                for (int i = 0; i < transform.childCount; i++)
                {
                    StartCoroutine(FlashRed(transform.GetChild(i).gameObject));
                }

                canTakeDamage = true;
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

    protected IEnumerator FlashRed(GameObject gObject)
    {
        if (!gObject.CompareTag("OrdagaExplosionTrigger"))
        {
            gObject.GetComponent<Image>().color = Color.red;
            yield return new WaitForSeconds(0.2F);
            gObject.GetComponent<Image>().color = Color.white; 
        }
    }
}
