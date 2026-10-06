using UnityEngine;

public class ResetModeTransform : MonoBehaviour
{
    [Header("Reset Target")]
    [SerializeField] private Transform target;

    private Transform referenceTransform;

    private Vector3 startOffsetFromReference;
    private Quaternion startRelativeRotation;
    private Vector3 startScale;

    private bool initialized;

    public void Initialize(
        Transform reference,
        Transform resetTarget)
    {
        referenceTransform = reference;

        target = resetTarget != null
            ? resetTarget
            : transform;

        if (referenceTransform == null)
        {
            Debug.LogWarning(
                $"{name}: Reference Transform is missing."
            );

            return;
        }

        // IMPORTANT:
        // Capture the ORIGINAL relationship now,
        // before MainUI can be moved.
        Vector3 worldOffset =
            target.position -
            referenceTransform.position;

        startOffsetFromReference =
            referenceTransform.InverseTransformDirection(
                worldOffset
            );

        startRelativeRotation =
            Quaternion.Inverse(
                referenceTransform.rotation
            ) * target.rotation;

        startScale =
            target.localScale;

        initialized = true;

        Debug.Log(
            $"{name}: Initial reference relationship cached."
        );
    }

    public void ResetToDefault()
    {
        if (!initialized)
        {
            Debug.LogWarning(
                $"{name}: Reset requested before initialization."
            );

            return;
        }

        // -------------------------------------------------
        // POSITION
        // -------------------------------------------------

        Vector3 currentWorldOffset =
            referenceTransform.TransformDirection(
                startOffsetFromReference
            );

        target.position =
            referenceTransform.position +
            currentWorldOffset;

        // -------------------------------------------------
        // ROTATION
        // -------------------------------------------------

        target.rotation =
            referenceTransform.rotation *
            startRelativeRotation;

        // -------------------------------------------------
        // SCALE
        // -------------------------------------------------

        target.localScale = startScale;

        Debug.Log(
            $"{name}: Reset relative to CURRENT MainUI."
        );
    }
}