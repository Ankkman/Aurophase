using System.Collections;
using UnityEngine;
using TMPro;

public class AurophaseIntro : MonoBehaviour
{
    [Header("Intro UI")]
    [SerializeField] private CanvasGroup line1;
    [SerializeField] private CanvasGroup line2;
    [SerializeField] private CanvasGroup title;

    [Header("Main Menu")]
    [SerializeField] private GameObject mainMenu;

    [Header("Intro Objects")]
    [SerializeField] private GameObject starEffect;

    [Header("Timing")]
    [SerializeField] private float fadeInTime = 1.2f;
    [SerializeField] private float displayTime = 2.0f;
    [SerializeField] private float fadeOutTime = 0.8f;
    [SerializeField] private float delayBetweenLines = 0.4f;

    private void Awake()
    {
        if (line1 != null)
            line1.alpha = 0f;

        if (line2 != null)
            line2.alpha = 0f;

        if (title != null)
            title.alpha = 0f;

        if (mainMenu != null)
            mainMenu.SetActive(false);

        if (starEffect != null)
            starEffect.SetActive(true);
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);

        yield return ShowText(line1);

        yield return new WaitForSeconds(delayBetweenLines);

        yield return ShowText(line2);

        yield return new WaitForSeconds(delayBetweenLines);

        yield return ShowText(title);

        yield return new WaitForSeconds(1.5f);

        if (mainMenu != null)
            mainMenu.SetActive(true);

        if (starEffect != null)
            starEffect.SetActive(false);

        gameObject.SetActive(false);
    }

    private IEnumerator ShowText(CanvasGroup text)
    {
        if (text == null)
            yield break;

        yield return Fade(text, 0f, 1f, fadeInTime);

        yield return new WaitForSeconds(displayTime);

        yield return Fade(text, 1f, 0f, fadeOutTime);
    }

    private IEnumerator Fade(
        CanvasGroup canvas,
        float from,
        float to,
        float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / duration);

            canvas.alpha = Mathf.Lerp(from, to, t);

            yield return null;
        }

        canvas.alpha = to;
    }
}