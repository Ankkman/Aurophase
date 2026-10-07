using UnityEngine;

public class BlackHoleSpawnAligner : MonoBehaviour
{
    [Header("Black Hole")]
    [SerializeField] private Transform blackHole;

    [Header("Spawn")]
    [SerializeField] private float eyeHeightOffset = 0f;

    [Header("Facing")]
    [SerializeField] private bool faceUser = true;

    private Camera vrCamera;

    public void AlignOnSpawn()
    {
        if (blackHole == null)
        {
            Debug.LogWarning(
                "BlackHoleSpawnAligner: Black Hole reference is missing."
            );
            return;
        }

        if (vrCamera == null)
            vrCamera = Camera.main;

        if (vrCamera == null)
        {
            Debug.LogWarning(
                "BlackHoleSpawnAligner: Main Camera not found."
            );
            return;
        }

        // --------------------------------------------------
        // 1. Put Black Hole at eye level
        // --------------------------------------------------

        Vector3 position = blackHole.position;

        position.y =
            vrCamera.transform.position.y +
            eyeHeightOffset;

        blackHole.position = position;

        // --------------------------------------------------
        // 2. Make the Black Hole perfectly upright
        // --------------------------------------------------

        if (faceUser)
        {
            Vector3 direction =
                vrCamera.transform.position -
                blackHole.position;

            // Ignore vertical difference.
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                blackHole.rotation =
                    Quaternion.LookRotation(
                        direction.normalized,
                        Vector3.up
                    );
            }
            else
            {
                // Fallback: keep current Y rotation.
                Vector3 currentEuler =
                    blackHole.eulerAngles;

                blackHole.rotation =
                    Quaternion.Euler(
                        0f,
                        currentEuler.y,
                        0f
                    );
            }
        }
        else
        {
            // Keep current horizontal direction,
            // but force the model perfectly upright.
            Vector3 currentEuler =
                blackHole.eulerAngles;

            blackHole.rotation =
                Quaternion.Euler(
                    0f,
                    currentEuler.y,
                    0f
                );
        }

        Debug.Log(
            "Black Hole aligned to eye level and upright."
        );
    }
}