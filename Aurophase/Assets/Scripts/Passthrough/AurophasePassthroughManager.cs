using System.Collections;
using UnityEngine;

public class AurophasePassthroughManager : MonoBehaviour
{
    public static AurophasePassthroughManager Instance { get; private set; }

    [Header("Passthrough")]
    [SerializeField] private OVRPassthroughLayer passthroughLayer;

    [Header("Normal Mode")]
    [Range(0f, 1f)]
    [SerializeField] private float normalOpacity = 0.45f;

    [Header("Ambient Stars")]
    [SerializeField] private GameObject surroundingStarsVFX;

    [Header("Transition")]
    [SerializeField] private float transitionDuration = 1.2f;

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

    // =========================================================
    // NORMAL MODE - IMMEDIATE
    // =========================================================

    public void SetNormalModeImmediate()
    {
        StopCurrentTransition();

        if (passthroughLayer != null)
        {
            // Keep passthrough running.
            passthroughLayer.enabled = true;

            // Dimmed real-world view.
            passthroughLayer.textureOpacity = normalOpacity;
        }

        if (surroundingStarsVFX != null)
        {
            surroundingStarsVFX.SetActive(true);
        }

        IsTransitioning = false;
    }

    // =========================================================
    // BLACK HOLE MODE - IMMEDIATE
    // =========================================================

    public void SetBlackHoleModeImmediate()
    {
        StopCurrentTransition();

        if (passthroughLayer != null)
        {
            // IMPORTANT:
            // Do NOT disable the passthrough system.
            // Just make the passthrough image invisible.
            passthroughLayer.enabled = true;
            passthroughLayer.textureOpacity = 0f;
        }

        if (surroundingStarsVFX != null)
        {
            surroundingStarsVFX.SetActive(false);
        }

        IsTransitioning = false;
    }

    // =========================================================
    // NORMAL MODE - SMOOTH
    // =========================================================

    public void SetNormalMode()
    {
        StopCurrentTransition();

        transitionCoroutine =
            StartCoroutine(TransitionToNormal());
    }

    // =========================================================
    // BLACK HOLE MODE - SMOOTH
    // =========================================================

    public void SetBlackHoleMode()
    {
        StopCurrentTransition();

        transitionCoroutine =
            StartCoroutine(TransitionToBlackHole());
    }

    // =========================================================
    // TRANSITION → BLACK HOLE
    // =========================================================

    private IEnumerator TransitionToBlackHole()
    {
        IsTransitioning = true;

        if (passthroughLayer == null)
        {
            FinishTransition();
            yield break;
        }

        // Keep the layer alive.
        passthroughLayer.enabled = true;

        float startOpacity =
            passthroughLayer.textureOpacity;

        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsed / transitionDuration
            );

            // Smooth transition instead of linear-looking fade.
            t = Mathf.SmoothStep(0f, 1f, t);

            passthroughLayer.textureOpacity =
                Mathf.Lerp(
                    startOpacity,
                    0f,
                    t
                );

            yield return null;
        }

        passthroughLayer.textureOpacity = 0f;

        // Stars disappear once we enter full black-hole space.
        if (surroundingStarsVFX != null)
        {
            surroundingStarsVFX.SetActive(false);
        }

        FinishTransition();
    }

    // =========================================================
    // TRANSITION → NORMAL
    // =========================================================

    private IEnumerator TransitionToNormal()
    {
        IsTransitioning = true;

        if (passthroughLayer == null)
        {
            FinishTransition();
            yield break;
        }

        passthroughLayer.enabled = true;

        // Bring our normal ambient stars back.
        if (surroundingStarsVFX != null)
        {
            surroundingStarsVFX.SetActive(true);
        }

        float startOpacity =
            passthroughLayer.textureOpacity;

        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsed / transitionDuration
            );

            t = Mathf.SmoothStep(0f, 1f, t);

            passthroughLayer.textureOpacity =
                Mathf.Lerp(
                    startOpacity,
                    normalOpacity,
                    t
                );

            yield return null;
        }

        passthroughLayer.textureOpacity =
            normalOpacity;

        FinishTransition();
    }

    // =========================================================
    // HELPERS
    // =========================================================

    private void FinishTransition()
    {
        IsTransitioning = false;
        transitionCoroutine = null;
    }

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