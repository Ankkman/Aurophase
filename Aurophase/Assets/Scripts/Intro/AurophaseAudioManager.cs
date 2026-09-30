using UnityEngine;

public class AurophaseAudioManager : MonoBehaviour
{
    public static AurophaseAudioManager Instance { get; private set; }

    [Header("Background Music")]
    [SerializeField] private AudioSource normalMusic;
    [SerializeField] private AudioSource blackHoleMusic;

    [Header("Effect Sources")]
    [SerializeField] private AudioSource uiAudioSource;
    [SerializeField] private AudioSource interactionAudioSource;

    [Header("UI Sounds")]
    [SerializeField] private AudioClip buttonClick;

    [Header("Interaction Sounds")]
    [SerializeField] private AudioClip grabSound;
    [SerializeField] private AudioClip dropSound;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        StartNormalMusic();
    }

    // =====================================================
    // NORMAL MUSIC
    // =====================================================

    public void StartNormalMusic()
    {
        if (blackHoleMusic != null &&
            blackHoleMusic.isPlaying)
        {
            blackHoleMusic.Stop();
        }

        if (normalMusic != null &&
            !normalMusic.isPlaying)
        {
            normalMusic.Play();
        }
    }

    // =====================================================
    // BLACK HOLE MUSIC
    // =====================================================

    public void StartBlackHoleMusic()
    {
        if (normalMusic != null &&
            normalMusic.isPlaying)
        {
            normalMusic.Stop();
        }

        if (blackHoleMusic != null &&
            !blackHoleMusic.isPlaying)
        {
            blackHoleMusic.Play();
        }
    }

    // =====================================================
    // UI CLICK
    // =====================================================

    public void PlayButtonClick()
    {
        if (uiAudioSource == null ||
            buttonClick == null)
            return;

        uiAudioSource.PlayOneShot(buttonClick);
    }

    // =====================================================
    // GRAB
    // =====================================================

    public void PlayGrab()
    {
        if (interactionAudioSource == null ||
            grabSound == null)
            return;

        interactionAudioSource.PlayOneShot(grabSound);
    }

    // =====================================================
    // DROP
    // =====================================================

    public void PlayDrop()
    {
        if (interactionAudioSource == null ||
            dropSound == null)
            return;

        interactionAudioSource.PlayOneShot(dropSound);
    }
}