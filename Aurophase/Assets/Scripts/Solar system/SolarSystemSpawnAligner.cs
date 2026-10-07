using UnityEngine;

public class SolarSystemSpawnAligner : MonoBehaviour
{
    [Header("Solar System")]
    [SerializeField] private Transform solarSystem;

    [Header("Spawn Height")]
    [SerializeField] private float eyeHeightOffset = -0.15f;

    [Header("Orientation")]
    [SerializeField] private Vector3 rotationOffset =
        new Vector3(90f, -90f, 0f);

    [SerializeField] private bool faceUser = true;

    private Camera vrCamera;

    public void AlignOnSpawn()
    {
        if (solarSystem == null)
        {
            Debug.LogWarning(
                "SolarSystemSpawnAligner: Solar System reference is missing."
            );
            return;
        }

        if (vrCamera == null)
            vrCamera = Camera.main;

        if (vrCamera == null)
        {
            Debug.LogWarning(
                "SolarSystemSpawnAligner: Main Camera not found."
            );
            return;
        }

        // --------------------------------------------------
        // 1. Eye-level position
        // --------------------------------------------------

        Vector3 position = solarSystem.position;

        position.y =
            vrCamera.transform.position.y +
            eyeHeightOffset;

        solarSystem.position = position;

        // --------------------------------------------------
        // 2. Calculate horizontal facing direction
        // --------------------------------------------------

        float userYaw = 0f;

        if (faceUser)
        {
            Vector3 direction =
                vrCamera.transform.position -
                solarSystem.position;

            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                userYaw =
                    Quaternion.LookRotation(
                        direction.normalized,
                        Vector3.up
                    ).eulerAngles.y;
            }
        }

        // --------------------------------------------------
        // 3. Apply fixed model orientation
        // --------------------------------------------------

        solarSystem.rotation =
            Quaternion.Euler(
                rotationOffset.x,
                userYaw + rotationOffset.y,
                rotationOffset.z
            );

        Debug.Log(
            "Solar System aligned to eye level and corrected orientation."
        );
    }
}