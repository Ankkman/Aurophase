using UnityEngine;
using TMPro;

public class PlanetInfoManager : MonoBehaviour
{
    public static PlanetInfoManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject infoCanvas;

    [SerializeField] private TMP_Text planetNameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text diameterText;
    [SerializeField] private TMP_Text orbitalPeriodText;
    [SerializeField] private TMP_Text distanceFromSunText;

    [Header("Canvas Transform")]
    [SerializeField] private Transform infoCanvasTransform;

    [Header("Panel Position")]
    [Tooltip("World-space X offset from the selected planet.")]
    [SerializeField] private float horizontalOffset = 0.65f;

    [Tooltip("World-space Y offset from the selected planet.")]
    [SerializeField] private float verticalOffset = 0.15f;

    [Tooltip("World-space Z offset from the selected planet.")]
    [SerializeField] private float depthOffset = 0f;

    [Header("Panel Facing")]
    [SerializeField] private bool alwaysFaceUser = true;

    [Tooltip("Enable if the panel appears visually backwards.")]
    [SerializeField] private bool flipPanel = false;

    [Header("Panel Scaling")]
    [SerializeField] private float referencePlanetScale = 0.05f;

    [Tooltip("Panel size when the planet is at or below its minimum reference scale.")]
    [SerializeField] private float minimumPanelScale = 0.55f;

    [Tooltip("Panel size when the planet reaches its normal/reference scale.")]
    [SerializeField] private float normalPanelScale = 1.0f;

    [Tooltip("Maximum size the information panel can reach.")]
    [SerializeField] private float maximumPanelScale = 1.0f;

    [Tooltip("Planet scale multiplier at which the panel reaches maximum size.")]
    [SerializeField] private float maximumPlanetScaleMultiplier = 2.0f;

    [SerializeField] private float panelScaleMultiplier = 1.0f;

    private PlanetInfoData selectedPlanet;
    private Camera mainCamera;

    // Original panel scale
    private Vector3 originalPanelScale;

    // Scale of the selected planet when information was opened
    private Vector3 selectedPlanetInitialScale;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        mainCamera = Camera.main;

        if (infoCanvasTransform != null)
        {
            originalPanelScale = infoCanvasTransform.localScale;
        }

        if (infoCanvas != null)
        {
            infoCanvas.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        if (selectedPlanet == null)
            return;

        UpdatePanelPositionAndRotation();
        UpdatePanelScale();
    }

    public void ShowPlanetInfo(PlanetInfoData data)
    {
        if (data == null)
            return;

        selectedPlanet = data;

        // -----------------------------------------
        // Store the planet's scale when selected
        // -----------------------------------------

        selectedPlanetInitialScale = selectedPlanet.transform.localScale;

        // -----------------------------------------
        // Fill UI
        // -----------------------------------------

        if (planetNameText != null)
            planetNameText.text = data.planetName;

        if (descriptionText != null)
            descriptionText.text = data.description;

        if (diameterText != null)
            diameterText.text = data.diameter;

        if (orbitalPeriodText != null)
            orbitalPeriodText.text = data.orbitalPeriod;

        if (distanceFromSunText != null)
            distanceFromSunText.text = data.distanceFromSun;

        // -----------------------------------------
        // Show panel
        // -----------------------------------------

        if (infoCanvas != null)
            infoCanvas.SetActive(true);

        UpdatePanelPositionAndRotation();
        UpdatePanelScale();
    }

    public void HidePlanetInfo()
    {
        selectedPlanet = null;

        // Restore original panel size
        if (infoCanvasTransform != null)
        {
            infoCanvasTransform.localScale = originalPanelScale;
        }

        if (infoCanvas != null)
            infoCanvas.SetActive(false);
    }

    private void UpdatePanelPositionAndRotation()
    {
        if (selectedPlanet == null || infoCanvasTransform == null)
            return;

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
            return;

        Transform planet = selectedPlanet.transform;

        // =========================================================
        // POSITION
        // =========================================================
        //
        // The panel follows the planet's WORLD POSITION.
        //
        // It does NOT use the camera's right/up vectors.
        // Therefore moving your head does not move the panel.
        // =========================================================

        Vector3 worldOffset = new Vector3(
            horizontalOffset,
            verticalOffset,
            depthOffset
        );

        Vector3 targetPosition =
            planet.position + worldOffset;

        infoCanvasTransform.position = targetPosition;


        // =========================================================
        // ROTATION
        // =========================================================
        //
        // The panel does NOT inherit planet rotation.
        //
        // It simply faces the user from its world position.
        // =========================================================

        // Scale UI according to planet size
        float planetScale = planet.lossyScale.x;

        float scaleFactor =
            (planetScale / referencePlanetScale) * panelScaleMultiplier;

        scaleFactor = Mathf.Clamp(
            scaleFactor,
            minimumPanelScale,
            maximumPanelScale
        );

        infoCanvasTransform.localScale =
            Vector3.one * scaleFactor;

        if (alwaysFaceUser)
        {
            Vector3 directionToCamera =
                mainCamera.transform.position -
                infoCanvasTransform.position;

            if (directionToCamera.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation =
                    Quaternion.LookRotation(
                        directionToCamera.normalized,
                        Vector3.up
                    );

                if (flipPanel)
                {
                    targetRotation *=
                        Quaternion.Euler(0f, 180f, 0f);
                }

                infoCanvasTransform.rotation =
                    targetRotation;
            }
        }
    }

    private void UpdatePanelScale()
    {
        if (selectedPlanet == null || infoCanvasTransform == null)
            return;

        Transform planet = selectedPlanet.transform;

        // ---------------------------------------------------------
        // Calculate how much the planet has changed relative to
        // the scale it had when the information panel appeared.
        // ---------------------------------------------------------

        float currentScale =
            planet.localScale.magnitude;

        float initialScale =
            selectedPlanetInitialScale.magnitude;

        if (initialScale <= 0.0001f)
            return;

        float relativeScale =
            currentScale / initialScale;

        // ---------------------------------------------------------
        // Convert planet scale into panel scale.
        //
        // 1.0x planet = normal panel size
        // 2.0x planet = maximum panel size
        // ---------------------------------------------------------

        float panelScale;

        if (relativeScale <= 1f)
        {
            // Planet is at its original size or smaller.
            panelScale = Mathf.Lerp(
                minimumPanelScale,
                normalPanelScale,
                relativeScale
            );
        }
        else
        {
            // Planet is larger than its original size.
            float t = Mathf.InverseLerp(
                1f,
                maximumPlanetScaleMultiplier,
                relativeScale
            );

            panelScale = Mathf.Lerp(
                normalPanelScale,
                maximumPanelScale,
                t
            );
        }

        // ---------------------------------------------------------
        // Absolute safety clamp.
        // ---------------------------------------------------------

        panelScale = Mathf.Clamp(
            panelScale,
            minimumPanelScale,
            maximumPanelScale
        );

        // ---------------------------------------------------------
        // Apply relative to the original Canvas scale.
        //
        // This prevents the panel from inheriting the planet's
        // actual scale.
        // ---------------------------------------------------------

        infoCanvasTransform.localScale =
            originalPanelScale * panelScale;
    }
}