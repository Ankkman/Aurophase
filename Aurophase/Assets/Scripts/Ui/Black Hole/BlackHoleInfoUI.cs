using UnityEngine;
using TMPro;

public class BlackHoleInfoUI : MonoBehaviour
{
    [Header("Black Hole")]
    [SerializeField] private Transform blackHole;

    [Header("UI")]
    [SerializeField] private GameObject infoCanvas;

    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;

    [SerializeField] private TMP_Text stat1Label;
    [SerializeField] private TMP_Text stat1Value;

    [SerializeField] private TMP_Text stat2Label;
    [SerializeField] private TMP_Text stat2Value;

    [SerializeField] private TMP_Text stat3Label;
    [SerializeField] private TMP_Text stat3Value;

    [Header("Position")]
    [SerializeField] private float horizontalOffset = 1.0f;
    [SerializeField] private float verticalOffset = 0.2f;
    [SerializeField] private float depthOffset = 0f;

    [Header("Facing")]
    [SerializeField] private bool faceUser = true;
    [SerializeField] private bool flipPanel = true;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;

        if (infoCanvas == null)
            infoCanvas = gameObject;

        // Black Hole information
        if (nameText != null)
            nameText.text = "BLACK HOLE";

        if (descriptionText != null)
        {
            descriptionText.text =
                "A region of spacetime where gravity is so strong " +
                "that nothing, not even light, can escape.";
        }

        if (stat1Label != null)
            stat1Label.text = "TYPE";

        if (stat1Value != null)
            stat1Value.text = "Stellar / Supermassive";

        if (stat2Label != null)
            stat2Label.text = "EVENT HORIZON";

        if (stat2Value != null)
            stat2Value.text = "Boundary of no return";

        if (stat3Label != null)
            stat3Label.text = "FORMATION";

        if (stat3Value != null)
            stat3Value.text = "Gravitational collapse";
    }

    private void Start()
    {
        if (blackHole == null)
        {
            Debug.LogWarning(
                "BlackHoleInfoUI: Black Hole reference is not assigned."
            );
        }

        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (blackHole == null)
            return;

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
            return;

        UpdatePosition();
        UpdateRotation();
    }

    private void UpdatePosition()
    {
        Vector3 worldOffset =
            blackHole.right * horizontalOffset +
            blackHole.up * verticalOffset +
            blackHole.forward * depthOffset;

        transform.position =
            blackHole.position + worldOffset;
    }

    private void UpdateRotation()
    {
        if (!faceUser)
            return;

        Vector3 direction =
            mainCamera.transform.position -
            transform.position;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(
                direction.normalized,
                Vector3.up
            );

        if (flipPanel)
        {
            targetRotation *=
                Quaternion.Euler(0f, 180f, 0f);
        }

        transform.rotation = targetRotation;
    }
}