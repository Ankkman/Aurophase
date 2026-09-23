using UnityEngine;
using TMPro;
using System.Collections;

public class IntroManager : MonoBehaviour
{
    [Header("Dependencies")]
    public PassthroughManager passthroughManager;
    public AudioClip ambientMusic;
    public ParticleSystem starParticles;
    public GameObject dockCanvas; // Added reference for the menu dock

    [Header("Settings")]
    public float logoDelay = 1.0f;
    public float textDisplayTime = 4.0f;
    public float textFadeDuration = 2.0f;
    public string introTextString = "There are billions of galaxies in the universe";

    private CanvasGroup textCanvasGroup;
    private AudioSource ambientAudioSource;

    void Start()
    {
        // 1. Hide the menu at the very start
        if (dockCanvas != null) dockCanvas.SetActive(false);

        SetupAudio();
        StartCoroutine(IntroSequence());
    }

    private IEnumerator IntroSequence()
    {
        yield return new WaitForSeconds(logoDelay);

        if (passthroughManager != null) passthroughManager.SetPassthroughDimmed(true);
        if (ambientAudioSource != null) ambientAudioSource.Play();
        if (starParticles != null) starParticles.Play();

        SetupIntroUI();
        yield return StartCoroutine(FadeOutRoutine());
    }

    private void SetupAudio()
    {
        if (ambientMusic == null) return;
        GameObject audioObj = new GameObject("AmbientAudio");
        audioObj.transform.parent = this.transform;
        ambientAudioSource = audioObj.AddComponent<AudioSource>();
        ambientAudioSource.clip = ambientMusic;
        ambientAudioSource.loop = true;
        ambientAudioSource.spatialBlend = 0f;
    }

    private void SetupIntroUI()
    {
        GameObject canvasObj = new GameObject("IntroCanvas");
        canvasObj.transform.position = new Vector3(0, 1.4f, 1.5f);
        
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        
        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(800, 200);
        canvasObj.transform.localScale = new Vector3(0.002f, 0.002f, 0.002f);

        textCanvasGroup = canvasObj.AddComponent<CanvasGroup>();

        GameObject textObj = new GameObject("IntroText");
        textObj.transform.SetParent(canvasObj.transform, false);

        TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();
        tmpText.text = introTextString;
        tmpText.fontSize = 36;
        tmpText.alignment = TextAlignmentOptions.Center;
        tmpText.color = Color.white;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(800, 200);
    }

    private IEnumerator FadeOutRoutine()
    {
        yield return new WaitForSeconds(textDisplayTime);

        float elapsedTime = 0f;
        while (elapsedTime < textFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            textCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / textFadeDuration);
            yield return null;
        }
        
        textCanvasGroup.alpha = 0f;

        // 2. Show the menu right after the intro text disappears
        if (dockCanvas != null) dockCanvas.SetActive(true);
    }
}