using UnityEngine;

public class ResetModeTransform : MonoBehaviour
{
    [Header("Reset Target")]
    [SerializeField] private Transform target;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector3 startScale;

    private bool initialized = false;

    private void Start()
    {
        if (target == null)
            target = transform;

        // Capture the transform after the scene has fully initialized.
        startPosition = target.position;
        startRotation = target.rotation;
        startScale = target.localScale;

        initialized = true;

        Debug.Log(
            $"[{name}] RESET START SAVED\n" +
            $"Position: {startPosition}\n" +
            $"Rotation: {startRotation.eulerAngles}\n" +
            $"Scale: {startScale}"
        );
    }

    public void ResetToDefault()
    {
        if (!initialized || target == null)
        {
            Debug.LogWarning(
                $"[{name}] Reset failed: target not initialized."
            );
            return;
        }

        Debug.Log(
            $"[{name}] RESETTING\n" +
            $"Position: {startPosition}\n" +
            $"Rotation: {startRotation.eulerAngles}\n" +
            $"Scale: {startScale}"
        );

        target.position = startPosition;
        target.rotation = startRotation;
        target.localScale = startScale;

        Debug.Log(
            $"[{name}] AFTER RESET\n" +
            $"Position: {target.position}\n" +
            $"Rotation: {target.rotation.eulerAngles}\n" +
            $"Scale: {target.localScale}"
        );
    }
}