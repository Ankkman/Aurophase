using System.Collections.Generic;
using UnityEngine;

public class PlanetModeResetter : MonoBehaviour
{
    [Header("Planet Mode")]
    [SerializeField] private Transform planetMode;

    private Transform referenceTransform;

    private Vector3 startOffsetFromReference;
    private Quaternion startRelativeRotation;

    private readonly Dictionary<PlanetHomeSlot, Transform> originalSlots =
        new Dictionary<PlanetHomeSlot, Transform>();

    private readonly Dictionary<PlanetHomeSlot, Vector3> originalScales =
        new Dictionary<PlanetHomeSlot, Vector3>();

    private readonly Dictionary<PlanetHomeSlot, Quaternion> originalRotations =
        new Dictionary<PlanetHomeSlot, Quaternion>();

    private bool initialized;

    // =====================================================
    // INITIALIZE FROM SPACE MODE CONTROLLER
    // =====================================================

    public void Initialize(
        Transform reference,
        Transform modeTransform)
    {
        referenceTransform = reference;

        planetMode = modeTransform != null
            ? modeTransform
            : transform;

        if (referenceTransform == null)
        {
            Debug.LogWarning(
                "PlanetModeResetter: Reference Transform missing."
            );

            return;
        }

        // -------------------------------------------------
        // CACHE ORIGINAL PLANET MODE OFFSET
        // -------------------------------------------------

        Vector3 worldOffset =
            planetMode.position -
            referenceTransform.position;

        startOffsetFromReference =
            referenceTransform.InverseTransformDirection(
                worldOffset
            );

        startRelativeRotation =
            Quaternion.Inverse(
                referenceTransform.rotation
            ) * planetMode.rotation;

        // -------------------------------------------------
        // CACHE ORIGINAL PLANETS + SLOTS
        // -------------------------------------------------

        CacheOriginalPlanetSetup();

        initialized = true;

        Debug.Log(
            $"PlanetModeResetter: Initialized with " +
            $"{originalSlots.Count} planets."
        );
    }

    // =====================================================
    // CACHE PLANET SETUP
    // =====================================================

    private void CacheOriginalPlanetSetup()
    {
        originalSlots.Clear();
        originalScales.Clear();
        originalRotations.Clear();

        PlanetHomeSlot[] allObjects =
            planetMode.GetComponentsInChildren<PlanetHomeSlot>(
                true
            );

        List<PlanetHomeSlot> slots =
            new List<PlanetHomeSlot>();

        // -------------------------------------------------
        // FIND ALL SLOT OBJECTS
        // -------------------------------------------------

        foreach (PlanetHomeSlot item in allObjects)
        {
            if (item == null)
                continue;

            if (!item.IsPlanet())
            {
                slots.Add(item);
            }
        }

        // -------------------------------------------------
        // FIND ORIGINAL SLOT FOR EACH PLANET
        // -------------------------------------------------

        foreach (PlanetHomeSlot planet in allObjects)
        {
            if (planet == null ||
                !planet.IsPlanet())
            {
                continue;
            }

            PlanetHomeSlot closestSlot = null;
            float closestDistance = Mathf.Infinity;

            foreach (PlanetHomeSlot slot in slots)
            {
                float distance =
                    Vector3.Distance(
                        planet.transform.position,
                        slot.transform.position
                    );

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestSlot = slot;
                }
            }

            if (closestSlot != null)
            {
                originalSlots[planet] =
                    closestSlot.transform;

                originalScales[planet] =
                    planet.transform.localScale;

                originalRotations[planet] =
                    planet.transform.localRotation;
            }
        }
    }

    // =====================================================
    // RESET ALL PLANETS
    // =====================================================

    public void ResetAllPlanets()
    {
        if (!initialized)
        {
            Debug.LogWarning(
                "PlanetModeResetter: Not initialized."
            );

            return;
        }

        // -------------------------------------------------
        // 1. MOVE PLANET MODE RELATIVE TO CURRENT MAIN UI
        // -------------------------------------------------

        ResetPlanetModeTransform();

        // -------------------------------------------------
        // 2. RESET EACH PLANET
        // -------------------------------------------------

        foreach (
            KeyValuePair<PlanetHomeSlot, Transform> pair
            in originalSlots
        )
        {
            PlanetHomeSlot planet = pair.Key;

            Transform originalSlotTransform =
                pair.Value;

            if (planet == null ||
                originalSlotTransform == null)
            {
                continue;
            }

            PlanetHomeSlot originalSlot =
                originalSlotTransform
                    .GetComponent<PlanetHomeSlot>();

            if (originalSlot == null)
                continue;

            // Remove from current slot/free space.
            planet.DetachFromCurrentSlot();

            // Restore position.
            planet.transform.position =
                originalSlot.transform.position;

            // Restore rotation.
            planet.transform.localRotation =
                originalRotations[planet];

            // Restore scale.
            planet.transform.localScale =
                originalScales[planet];

            // Put back into original slot.
            planet.AssignToSlot(originalSlot);
        }

        // -------------------------------------------------
        // 3. UPDATE SLOT VISUALS
        // -------------------------------------------------

        foreach (
            KeyValuePair<PlanetHomeSlot, Transform> pair
            in originalSlots
        )
        {
            PlanetHomeSlot slot =
                pair.Value.GetComponent<PlanetHomeSlot>();

            if (slot != null)
            {
                slot.UpdateSlotVisual();
            }
        }

        Debug.Log(
            "Planet Mode reset complete."
        );
    }

    // =====================================================
    // RESET PLANET MODE TRANSFORM
    // =====================================================

    private void ResetPlanetModeTransform()
    {
        Vector3 currentWorldOffset =
            referenceTransform.TransformDirection(
                startOffsetFromReference
            );

        planetMode.position =
            referenceTransform.position +
            currentWorldOffset;

        planetMode.rotation =
            referenceTransform.rotation *
            startRelativeRotation;
    }
}