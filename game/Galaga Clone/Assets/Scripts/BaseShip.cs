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
                if (CompareTag("Player"))
                {
                    StartCoroutine(PlayerFlashRed());
                }
                else
                {
                    StartCoroutine(FlashRed());
                    canTakeDamage = true;
                }
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

    protected IEnumerator FlashRed()
    {
        ChangeShipColor(Color.red);
        yield return new WaitForSeconds(0.2F);
        ChangeShipColor(Color.white);
    }

    private IEnumerator PlayerFlashRed()
    {
        ChangeShipColor(Color.red);
        yield return new WaitForSeconds(0.2F);
        ChangeShipColor(Color.white);
        canTakeDamage = true;
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
}
