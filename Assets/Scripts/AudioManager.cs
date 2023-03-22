using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [HideInInspector] public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioClip[] keyboardSounds;
    [SerializeField] private AudioClip[] incompleteSounds;

    private AudioSource audioSource;

    public void PlayKeyboard()
    {
        audioSource.PlayOneShot(keyboardSounds[Random.Range(0, keyboardSounds.Length)]);
    }

    public void PlayIncomplete()
    {
        audioSource.PlayOneShot(incompleteSounds[Random.Range(0, incompleteSounds.Length)]);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }
}
