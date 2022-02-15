using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAudio : MonoBehaviour
{   
    public AudioClip clickSound;
    private AudioSource audioSource;

    void OnEnable()
    {
        audioSource = GetComponents<AudioSource>()[1];
    }

    public void PlayClickSound()
    {
        audioSource.PlayOneShot(clickSound);
    }
}
