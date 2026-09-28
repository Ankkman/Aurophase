using UnityEngine;

public class PlanetInfoFollower : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform targetPlanet;
    [SerializeField] private Camera vrCamera;

    [Header("Position")]
    [SerializeField] private Vector3 worldOffset = new Vector3(0.8f, 0.25f, 0f);

    [Header("Billboarding")]
    [SerializeField] private bool faceCamera = true;

    [Header("Smoothing")]
    [SerializeField] private float positionSmooth = 12f;
    [SerializeField] private float rotationSmooth = 12f;

    private bool following;

    private void Awake()
    {
        if (vrCamera == null)
            vrCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (!following || targetPlanet == null)
            return;

        if (vrCamera == null)
            vrCamera = Camera.main;

        // -----------------------------------------
        // 1. Follow ONLY the planet's POSITION
        // -----------------------------------------

        Vector3 desiredPosition =
            targetPlanet.position + worldOffset;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            positionSmooth * Time.deltaTime
        );

        // -----------------------------------------
        // 2. Do NOT inherit planet rotation
        // -----------------------------------------

        if (faceCamera && vrCamera != null)
        {
            Vector3 directionToCamera =
                vrCamera.transform.position - transform.position;

            if (directionToCamera.sqrMagnitude > 0.001f)
            {
                Quaternion desiredRotation =
                    Quaternion.LookRotation(
                        directionToCamera.normalized,
                        Vector3.up
                    );

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    desiredRotation,
                    rotationSmooth * Time.deltaTime
                );
            }
        }

        // -----------------------------------------
        // IMPORTANT:
        // We never copy targetPlanet.localScale.
        // Therefore the info panel does NOT scale
        // with the planet.
        // -----------------------------------------
    }

    public void ShowForPlanet(Transform planet)
    {
        targetPlanet = planet;
        following = true;

        if (vrCamera == null)
            vrCamera = Camera.main;

        // Snap immediately instead of waiting for smoothing
        if (targetPlanet != null)
        {
            transform.position =
                targetPlanet.position + worldOffset;
        }
    }

    public void Hide()
    {
        following = false;
        targetPlanet = null;
    }

    public Transform GetTargetPlanet()
    {
        return targetPlanet;
    }
}