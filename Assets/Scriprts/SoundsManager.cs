using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Sound Clips")]
    public AudioClip airHitSound;
    public AudioClip enemyHitSound;
    public AudioClip playerHitSound;

    [Header("Audio Settings")]
    public float volume = 1.0f;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlayAirHit()
    {
        if (airHitSound != null)
        {
            audioSource.PlayOneShot(airHitSound, volume);
        }
    }

    public void PlayEnemyHit()
    {
        if (enemyHitSound != null)
        {
            audioSource.PlayOneShot(enemyHitSound, volume);
        }
    }

    public void PlayPlayerHit()
    {
        if (playerHitSound != null)
        {
            audioSource.PlayOneShot(playerHitSound, volume);
        }
    }

    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        audioSource.volume = volume;
    }
}