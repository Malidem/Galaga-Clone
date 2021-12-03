using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public AudioClip explosion;

    [HideInInspector]
    public GameObject parent;

    private AudioSource audioSrc;

    void Update()
    {
        if (parent != null)
        {
            transform.position = parent.transform.position;
        }
    }

    public IEnumerator Die()
    {
        audioSrc = gameObject.GetComponent<AudioSource>();
        audioSrc.PlayOneShot(explosion);
        yield return new WaitForSeconds(0.6F);
        Destroy(gameObject);
    }
}
