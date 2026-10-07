using UnityEngine;

public class BlackHoleVisualController : MonoBehaviour
{
    [Header("Black Hole Parts")]
    [SerializeField] private Transform accretionDisk;
    [SerializeField] private Transform einsteinRing;
    [SerializeField] private Transform einsteinRing2;
    [SerializeField] private Transform glowingPart;

    [Header("Rotation")]
    [SerializeField] private float accretionDiskSpeed = 18f;
    [SerializeField] private float einsteinRingSpeed = 5f;
    [SerializeField] private float einsteinRing2Speed = -3f;

    [Header("Glow Pulse")]
    [SerializeField] private bool enableGlowPulse = true;
    [SerializeField] private float pulseSpeed = 1.5f;
    [SerializeField] private float pulseAmount = 0.035f;

    private Vector3 originalGlowScale;

    private void Start()
    {
        if (glowingPart != null)
        {
            originalGlowScale = glowingPart.localScale;
        }
    }

    private void Update()
    {
        AnimateRotation();
        AnimateGlow();
    }

    private void AnimateRotation()
    {
        if (accretionDisk != null)
        {
            accretionDisk.Rotate(
                Vector3.up,
                accretionDiskSpeed * Time.deltaTime,
                Space.Self
            );
        }

        if (einsteinRing != null)
        {
            einsteinRing.Rotate(
                Vector3.up,
                einsteinRingSpeed * Time.deltaTime,
                Space.Self
            );
        }

        if (einsteinRing2 != null)
        {
            einsteinRing2.Rotate(
                Vector3.up,
                einsteinRing2Speed * Time.deltaTime,
                Space.Self
            );
        }
    }

    private void AnimateGlow()
    {
        if (!enableGlowPulse || glowingPart == null)
            return;

        float pulse =
            1f +
            Mathf.Sin(Time.time * pulseSpeed) *
            pulseAmount;

        glowingPart.localScale =
            originalGlowScale * pulse;
    }
}