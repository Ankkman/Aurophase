using System.Collections;
using UnityEngine;

public class VRUIStartPosition : MonoBehaviour
{
    [Header("Placement")]
    [SerializeField] private float distanceFromHead = 1.5f;
    [SerializeField] private float verticalOffset = 0f;

    [Header("Rotation")]
    [SerializeField] private bool faceUser = true;

    [Header("Startup")]
    [SerializeField] private float placementDelay = 0.2f;

    private Camera mainCamera;
    private bool hasPlaced;

    private void OnEnable()
    {
        hasPlaced = false;
        StartCoroutine(PlaceUIWhenXRReady());
    }

    private IEnumerator PlaceUIWhenXRReady()
    {
        // Give XR time to initialize
        yield return null;
        yield return new WaitForSeconds(placementDelay);

        // Try a few frames to find the XR camera
        for (int i = 0; i < 30; i++)
        {
            mainCamera = Camera.main;

            if (mainCamera != null)
            {
                PlaceInFrontOfUser();
                yield break;
            }

            yield return null;
        }

        Debug.LogWarning("VRUIStartPosition: Main Camera not found.");
    }

    private void PlaceInFrontOfUser()
    {
        if (mainCamera == null || hasPlaced)
            return;

        Transform head = mainCamera.transform;

        // Only use the horizontal direction the user is facing.
        // This prevents the UI from being placed above/below them
        // if the headset is tilted.
        Vector3 forward = head.forward;
        forward.y = 0f;

        if (forward.sqrMagnitude < 0.001f)
            forward = Vector3.forward;

        forward.Normalize();

        // Position UI in front of the headset
        transform.position =
            head.position
            + forward * distanceFromHead
            + Vector3.up * verticalOffset;

        // Face the user
        if (faceUser)
        {
            Vector3 direction =
                head.position - transform.position;

            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                transform.rotation =
                    Quaternion.LookRotation(
                        -direction.normalized,
                        Vector3.up
                    );
            }
        }

        hasPlaced = true;
    }
}