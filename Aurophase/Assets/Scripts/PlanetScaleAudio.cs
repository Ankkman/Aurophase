using UnityEngine;

public class PlanetScaleAudio : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float scaleThreshold = 0.003f;

    [Header("Sound")]
    [SerializeField] private float soundCooldown = 0.12f;

    private float lastScaleMagnitude;
    private float cooldownTimer;

    private void Start()
    {
        lastScaleMagnitude =
            transform.localScale.magnitude;
    }

    private void Update()
    {
        float currentScaleMagnitude =
            transform.localScale.magnitude;

        float difference =
            currentScaleMagnitude -
            lastScaleMagnitude;

        cooldownTimer -= Time.deltaTime;

        if (Mathf.Abs(difference) >= scaleThreshold)
        {
            if (cooldownTimer <= 0f)
            {
                bool scalingUp = difference > 0f;

                if (AurophaseAudioManager.Instance != null)
                {
                    AurophaseAudioManager.Instance
                        .PlayPlanetScale(scalingUp);
                }

                cooldownTimer = soundCooldown;
            }
        }

        lastScaleMagnitude = currentScaleMagnitude;
    }
}