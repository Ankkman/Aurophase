using System.Collections;
using UnityEngine;

public class AurophasePassthroughManager : MonoBehaviour
{
    public static AurophasePassthroughManager Instance { get; private set; }

    [Header("Passthrough")]
    [SerializeField] private OVRPassthroughLayer passthroughLayer;

    [Range(0f, 1f)]
    [SerializeField] private float normalOpacity = 0.45f;

    [Header("Ambient Stars")]
    [SerializeField] private GameObject surroundingStarsVFX;

    [Header("Environment Transition")]
    [SerializeField] private float transitionDuration = 1.2f;

    [Tooltip("Opacity used during the black-hole state.")]
    [Range(0f, 0.2f)]
    [SerializeField] private float blackHoleOpacity = 0.02f;

    private Coroutine transitionCoroutine;

    public bool IsTransitioning { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        SetNormalModeImmediate();
    }

    // =====================================================
    // NORMAL MODE - IMMEDIATE
    // =====================================================

    public void SetNormalModeImmediate()
    {
        StopCurrentTransition();

        IsTransitioning = false;

        if (passthroughLayer != null)
        {
            passthroughLayer.enabled = true;
            passthroughLayer.textureOpacity = normalOpacity;
        }

        if (surroundingStarsVFX != null)
        {
            surroundingStarsVFX.SetActive(true);
        }
    }

    // =====================================================
    // BLACK HOLE MODE - IMMEDIATE
    // =====================================================

    public void SetBlackHoleModeImmediate()
    {
        StopCurrentTransition();

        IsTransitioning = false;

        if (passthroughLayer != null)
        {
            passthroughLayer.enabled = false;
        }

        if (surroundingStarsVFX != null)
        {
            surroundingStarsVFX.SetActive(false);
        }
    }

    // =====================================================
    // NORMAL MODE - SMOOTH
    // =====================================================

    public void SetNormalMode()
    {
        StopCurrentTransition();

        transitionCoroutine =
            StartCoroutine(TransitionToNormal());
    }

    // =====================================================
    // BLACK HOLE MODE - SMOOTH
    // =====================================================

    public void SetBlackHoleMode()
    {
        StopCurrentTransition();

        transitionCoroutine =
            StartCoroutine(TransitionToBlackHole());
    }

    // =====================================================
    // TRANSITION TO BLACK HOLE
    // =====================================================

    private IEnumerator TransitionToBlackHole()
    {
        IsTransitioning = true;

        if (passthroughLayer == null)
        {
            FinishTransition();
            yield break;
        }

        // -------------------------------------------------
        // 1. Make sure passthrough is visible
        // -------------------------------------------------

        passthroughLayer.enabled = true;

        float startOpacity =
            passthroughLayer.textureOpacity;

        float elapsed = 0f;

        // -------------------------------------------------
        // 2. Fade normal passthrough -> black
        // -------------------------------------------------

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed / transitionDuration
                );

            float smoothT =
                Mathf.SmoothStep(0f, 1f, t);

            passthroughLayer.textureOpacity =
                Mathf.Lerp(
                    startOpacity,
                    blackHoleOpacity,
                    smoothT
                );

            yield return null;
        }

        // -------------------------------------------------
        // 3. Final dark state
        // -------------------------------------------------

        passthroughLayer.textureOpacity =
            blackHoleOpacity;

        // -------------------------------------------------
        // 4. Disable passthrough completely
        // -------------------------------------------------

        passthroughLayer.enabled = false;

        // -------------------------------------------------
        // 5. NOW remove stars
        // -------------------------------------------------

        if (surroundingStarsVFX != null)
        {
            surroundingStarsVFX.SetActive(false);
        }

        FinishTransition();
    }

    // =====================================================
    // TRANSITION TO NORMAL
    // =====================================================

    private IEnumerator TransitionToNormal()
    {
        IsTransitioning = true;

        if (passthroughLayer == null)
        {
            FinishTransition();
            yield break;
        }

        // -------------------------------------------------
        // 1. Make passthrough available again
        // -------------------------------------------------

        passthroughLayer.enabled = true;

        // Start completely dark.
        passthroughLayer.textureOpacity =
            blackHoleOpacity;

        float elapsed = 0f;

        // -------------------------------------------------
        // 2. Fade black -> normal dimmed passthrough
        // -------------------------------------------------

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed / transitionDuration
                );

            float smoothT =
                Mathf.SmoothStep(0f, 1f, t);

            passthroughLayer.textureOpacity =
                Mathf.Lerp(
                    blackHoleOpacity,
                    normalOpacity,
                    smoothT
                );

            yield return null;
        }

        // -------------------------------------------------
        // 3. Final normal passthrough
        // -------------------------------------------------

        passthroughLayer.textureOpacity =
            normalOpacity;

        // -------------------------------------------------
        // 4. NOW bring stars back
        // -------------------------------------------------

        if (surroundingStarsVFX != null)
        {
            surroundingStarsVFX.SetActive(true);
        }

        FinishTransition();
    }

    // =====================================================
    // FINISH
    // =====================================================

    private void FinishTransition()
    {
        IsTransitioning = false;
        transitionCoroutine = null;
    }

    // =====================================================
    // STOP CURRENT TRANSITION
    // =====================================================

    private void StopCurrentTransition()
    {
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
            transitionCoroutine = null;
        }

        IsTransitioning = false;
    }
}