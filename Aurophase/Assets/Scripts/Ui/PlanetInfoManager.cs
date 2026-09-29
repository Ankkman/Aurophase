using UnityEngine;
using TMPro;
using System.Collections.Generic;

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

    // =========================================================
    // DEFAULT PANEL POSITION
    // =========================================================

    [Header("Default Panel Position")]

    [Tooltip("Default horizontal distance from the planet center.")]
    [SerializeField] private float defaultHorizontalOffset = 0.13f;

    [Tooltip("Default vertical distance from the planet center.")]
    [SerializeField] private float defaultVerticalOffset = 0f;

    [Tooltip("Default depth offset from the planet.")]
    [SerializeField] private float defaultDepthOffset = 0f;

    // =========================================================
    // DEFAULT PANEL SCALE
    // =========================================================

    [Header("Default Panel Scale")]

    [Tooltip("Panel scale when the planet is at its default size.")]
    [SerializeField] private float defaultPanelScale = 0.3f;

    // =========================================================
    // BEHAVIOUR
    // =========================================================

    [Header("Scaling Behaviour")]

    [Tooltip("Scale the panel position together with the planet.")]
    [SerializeField] private bool scalePositionWithPlanet = true;

    [Tooltip("Scale the panel size together with the planet.")]
    [SerializeField] private bool scalePanelWithPlanet = true;

    // =========================================================
    // PANEL FACING
    // =========================================================

    [Header("Panel Facing")]

    [SerializeField] private bool alwaysFaceUser = true;

    [Tooltip("Enable if the panel appears visually backwards.")]
    [SerializeField] private bool flipPanel = true;

    // =========================================================
    // INTERNAL
    // =========================================================

    private PlanetInfoData selectedPlanet;

    private Camera mainCamera;

    private Vector3 originalPanelScale;

    // Stores the default scale of each planet.
    // This prevents re-grabbing from resetting the reference.
    private Dictionary<Transform, Vector3> planetReferenceScales =
        new Dictionary<Transform, Vector3>();

    // Direction chosen when the panel is first shown.
    // This prevents the panel from following the headset.
    private Vector3 fixedPanelDirection;

    // =========================================================
    // UNITY
    // =========================================================

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
            originalPanelScale =
                infoCanvasTransform.localScale;
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

        UpdatePanel();
    }

    // =========================================================
    // SHOW PLANET INFO
    // =========================================================

    public void ShowPlanetInfo(PlanetInfoData data)
    {
        if (data == null)
            return;

        selectedPlanet = data;

        Transform planet =
            selectedPlanet.transform;

        if (mainCamera == null)
            mainCamera = Camera.main;

        // =====================================================
        // SAVE DEFAULT PLANET SCALE
        // =====================================================
        //
        // IMPORTANT:
        //
        // Only save this the FIRST time this planet is selected.
        //
        // Therefore:
        //
        // Default Earth = reference
        //
        // Grab Earth → scale Earth → release
        //
        // Grab Earth again
        //
        // The reference does NOT change.
        //

        if (!planetReferenceScales.ContainsKey(planet))
        {
            planetReferenceScales.Add(
                planet,
                planet.localScale
            );
        }

        // =====================================================
        // DETERMINE PANEL SIDE
        // =====================================================

        if (mainCamera != null)
        {
            fixedPanelDirection =
                mainCamera.transform.right;

            // Keep direction horizontal.
            fixedPanelDirection.y = 0f;

            if (fixedPanelDirection.sqrMagnitude < 0.001f)
            {
                fixedPanelDirection = Vector3.right;
            }
            else
            {
                fixedPanelDirection.Normalize();
            }
        }
        else
        {
            fixedPanelDirection = Vector3.right;
        }

        // =====================================================
        // FILL TEXT
        // =====================================================

        if (planetNameText != null)
            planetNameText.text =
                data.planetName;

        if (descriptionText != null)
            descriptionText.text =
                data.description;

        if (diameterText != null)
            diameterText.text =
                data.diameter;

        if (orbitalPeriodText != null)
            orbitalPeriodText.text =
                data.orbitalPeriod;

        if (distanceFromSunText != null)
            distanceFromSunText.text =
                data.distanceFromSun;

        // =====================================================
        // SHOW
        // =====================================================

        if (infoCanvas != null)
            infoCanvas.SetActive(true);

        UpdatePanel();
    }

    // =========================================================
    // HIDE
    // =========================================================

    public void HidePlanetInfo()
    {
        selectedPlanet = null;

        if (infoCanvasTransform != null)
        {
            infoCanvasTransform.localScale = originalPanelScale;
        }

        if (infoCanvas != null)
        {
            infoCanvas.SetActive(false);
        }
    }

    public void HidePlanetInfoForPlanet(PlanetInfoData planetData)
    {
        if (planetData == null)
            return;

        // Only hide the panel if this is the planet currently
        // being displayed.
        if (selectedPlanet == planetData)
        {
            HidePlanetInfo();
        }
    }

    // =========================================================
    // MAIN PANEL UPDATE
    // =========================================================

    private void UpdatePanel()
    {
        if (selectedPlanet == null ||
            infoCanvasTransform == null)
            return;

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
            return;

        Transform planet =
            selectedPlanet.transform;

        // =====================================================
        // GET DEFAULT PLANET SCALE
        // =====================================================

        if (!planetReferenceScales.TryGetValue(
                planet,
                out Vector3 referenceScale))
        {
            referenceScale =
                planet.localScale;

            planetReferenceScales.Add(
                planet,
                referenceScale
            );
        }

        // =====================================================
        // CALCULATE RELATIVE PLANET SCALE
        // =====================================================
        //
        // Example:
        //
        // Default = 0.05
        // Current = 0.10
        //
        // Relative = 2
        //
        // Therefore:
        //
        // Panel position = 2×
        // Panel size     = 2×
        //

        float referenceMagnitude =
            referenceScale.magnitude;

        float currentMagnitude =
            planet.localScale.magnitude;

        if (referenceMagnitude <= 0.00001f)
            return;

        float relativeScale =
            currentMagnitude /
            referenceMagnitude;

        // =====================================================
        // POSITION
        // =====================================================

        float positionMultiplier =
            scalePositionWithPlanet
                ? relativeScale
                : 1f;

        Vector3 offset =
            fixedPanelDirection *
            defaultHorizontalOffset *
            positionMultiplier;

        offset +=
            Vector3.up *
            defaultVerticalOffset *
            positionMultiplier;

        offset +=
            Vector3.forward *
            defaultDepthOffset *
            positionMultiplier;

        infoCanvasTransform.position =
            planet.position + offset;

        // =====================================================
        // PANEL SCALE
        // =====================================================

        float panelScale =
            scalePanelWithPlanet
                ? defaultPanelScale * relativeScale
                : defaultPanelScale;

        infoCanvasTransform.localScale =
            originalPanelScale * panelScale;

        // =====================================================
        // FACE USER
        // =====================================================

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
                        Quaternion.Euler(
                            0f,
                            180f,
                            0f
                        );
                }

                infoCanvasTransform.rotation =
                    targetRotation;
            }
        }
    }

    public void HidePlanetInfoFor(PlanetInfoData planet)
    {
        if (planet == null)
            return;

        // Only hide the panel if the planet being returned
        // is currently the planet whose information is displayed.
        if (selectedPlanet != planet)
            return;

        HidePlanetInfo();
    }
}