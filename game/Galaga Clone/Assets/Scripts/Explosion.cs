using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public AudioClip explosion;

    [HideInInspector]
    public GameObject parent;

    private AudioSource audioSource;

    void Update()
    {
        if (parent != null)
        {
            transform.position = parent.transform.position;
        }
    }

    public IEnumerator Die()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.volume = PlayerPrefs.GetFloat(DataBaseManager.Prefs.soundVolume);
        audioSource.PlayOneShot(explosion);
        yield return new WaitForSeconds(0.6F);
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}
