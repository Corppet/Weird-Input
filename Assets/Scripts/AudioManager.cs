using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [HideInInspector] public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioClip[] keyboardSounds;
    [SerializeField] private AudioClip[] switchSounds;
    [SerializeField] private AudioClip[] failSounds;

    private AudioSource audioSource;

    public void PlayKeyboard()
    {
        audioSource.PlayOneShot(keyboardSounds[Random.Range(0, keyboardSounds.Length)]);
    }

    public void PlaySwitch()
    {
        audioSource.PlayOneShot(switchSounds[Random.Range(0, switchSounds.Length)]);
    }

    public void PlayFail()
    {
        audioSource.PlayOneShot(failSounds[Random.Range(0, failSounds.Length)]);
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
