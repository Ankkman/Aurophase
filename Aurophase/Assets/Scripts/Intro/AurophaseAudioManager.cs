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

    [Header("Normal Interaction Sounds")]
    [SerializeField] private AudioClip grabSound;
    [SerializeField] private AudioClip dropSound;

    [Header("Planet Slot Sounds")]
    [SerializeField] private AudioClip planetSlotGrabSound;
    [SerializeField] private AudioClip planetSlotDropSound;

    [Header("Planet Scaling")]
    [SerializeField] private AudioClip planetScaleSound;

    [Tooltip("Pitch when planet is scaling UP.")]
    [SerializeField] private float scaleUpPitch = 1.08f;

    [Tooltip("Pitch when planet is scaling DOWN.")]
    [SerializeField] private float scaleDownPitch = 0.92f;

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
    // NORMAL GRAB
    // =====================================================

    public void PlayGrab()
    {
        if (interactionAudioSource == null ||
            grabSound == null)
            return;

        interactionAudioSource.PlayOneShot(grabSound);
    }

    // =====================================================
    // NORMAL DROP
    // =====================================================

    public void PlayDrop()
    {
        if (interactionAudioSource == null ||
            dropSound == null)
            return;

        interactionAudioSource.PlayOneShot(dropSound);
    }

    // =====================================================
    // PLANET TAKEN FROM SLOT
    // =====================================================

    public void PlayPlanetSlotGrab()
    {
        if (interactionAudioSource == null ||
            planetSlotGrabSound == null)
            return;

        interactionAudioSource.PlayOneShot(
            planetSlotGrabSound
        );
    }

    // =====================================================
    // PLANET RETURNED TO SLOT
    // =====================================================

    public void PlayPlanetSlotDrop()
    {
        if (interactionAudioSource == null ||
            planetSlotDropSound == null)
            return;

        interactionAudioSource.PlayOneShot(
            planetSlotDropSound
        );
    }

    // =====================================================
    // PLANET SCALE
    // =====================================================

    public void PlayPlanetScale(bool scalingUp)
    {
        if (interactionAudioSource == null ||
            planetScaleSound == null)
            return;

        float oldPitch = interactionAudioSource.pitch;

        interactionAudioSource.pitch =
            scalingUp ? scaleUpPitch : scaleDownPitch;

        interactionAudioSource.PlayOneShot(
            planetScaleSound
        );

        interactionAudioSource.pitch = oldPitch;
    }
}