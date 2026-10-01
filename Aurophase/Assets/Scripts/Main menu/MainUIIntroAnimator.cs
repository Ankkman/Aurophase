using System.Collections;
using UnityEngine;

public class MainUIIntroAnimator : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private float duration = 0.9f;

    [SerializeField] private float startingScale = 0.92f;

    [SerializeField] private AnimationCurve animationCurve =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private CanvasGroup canvasGroup;
    private Vector3 originalScale;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        StartCoroutine(PlayIntro());
    }

    private IEnumerator PlayIntro()
    {
        transform.localScale =
            originalScale * startingScale;

        canvasGroup.alpha = 0f;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            float smoothT =
                animationCurve.Evaluate(t);

            canvasGroup.alpha = smoothT;

            transform.localScale =
                Vector3.Lerp(
                    originalScale * startingScale,
                    originalScale,
                    smoothT
                );

            yield return null;
        }

        canvasGroup.alpha = 1f;
        transform.localScale = originalScale;
    }
}