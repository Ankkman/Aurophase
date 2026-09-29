using System.Collections.Generic;
using UnityEngine;

public class PlanetSlotManager : MonoBehaviour
{
    public static PlanetSlotManager Instance { get; private set; }

    [Header("Slot Settings")]
    [SerializeField] private float placementDistance = 0.35f;

    private readonly List<PlanetHomeSlot> slots =
        new List<PlanetHomeSlot>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // =========================================================
    // REGISTER / UNREGISTER
    // =========================================================

    public void RegisterSlot(PlanetHomeSlot slot)
    {
        if (slot == null)
            return;

        if (!slots.Contains(slot))
            slots.Add(slot);

        slot.UpdateSlotVisual();
    }

    public void UnregisterSlot(PlanetHomeSlot slot)
    {
        if (slot == null)
            return;

        slots.Remove(slot);
    }

    // =========================================================
    // IMPORTANT:
    // INITIALIZE A PLANET THAT IS ALREADY SITTING IN A SLOT
    // =========================================================

    public void InitializePlanetIfAtSlot(PlanetHomeSlot planet)
    {
        if (planet == null || !planet.IsPlanet())
            return;

        // Already assigned.
        if (planet.CurrentSlot != null)
            return;

        PlanetHomeSlot nearestSlot =
            FindNearestSlotRegardlessOfOccupancy(
                planet.transform.position
            );

        if (nearestSlot == null)
            return;

        float distance = Vector3.Distance(
            planet.transform.position,
            nearestSlot.transform.position
        );

        if (distance > placementDistance)
            return;

        // If another planet already owns this slot,
        // do NOT steal it.
        if (!nearestSlot.IsEmpty)
        {
            Debug.LogWarning(
                $"{planet.name} is near {nearestSlot.name}, " +
                "but that slot is already occupied."
            );

            return;
        }

        planet.AssignToSlot(nearestSlot);

        planet.transform.position =
            nearestSlot.transform.position;

        planet.transform.rotation =
            nearestSlot.transform.rotation;

        planet.transform.localScale =
            planet.GetDefaultPlanetScale();

        nearestSlot.UpdateSlotVisual();
    }

    // =========================================================
    // FIND NEAREST SLOT
    // ONLY EMPTY SLOTS ARE CONSIDERED
    // =========================================================

    public PlanetHomeSlot FindNearestEmptySlot(
        Vector3 position)
    {
        PlanetHomeSlot nearest = null;
        float nearestDistance = float.MaxValue;

        foreach (PlanetHomeSlot slot in slots)
        {
            if (slot == null)
                continue;

            // THIS IS THE IMPORTANT FIX.
            if (!slot.IsEmpty)
                continue;

            float distance = Vector3.Distance(
                position,
                slot.transform.position
            );

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = slot;
            }
        }

        if (nearest == null)
            return null;

        if (nearestDistance <= placementDistance)
            return nearest;

        return null;
    }

    // =========================================================
    // FIND NEAREST SLOT FOR INITIALIZATION
    // =========================================================

    private PlanetHomeSlot FindNearestSlotRegardlessOfOccupancy(
        Vector3 position)
    {
        PlanetHomeSlot nearest = null;
        float nearestDistance = float.MaxValue;

        foreach (PlanetHomeSlot slot in slots)
        {
            if (slot == null)
                continue;

            float distance = Vector3.Distance(
                position,
                slot.transform.position
            );

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = slot;
            }
        }

        if (nearest == null)
            return null;

        if (nearestDistance <= placementDistance)
            return nearest;

        return null;
    }

    // =========================================================
    // TRY PLACE PLANET
    // =========================================================

    public bool TryPlacePlanet(
        PlanetHomeSlot planet)
    {
        if (planet == null)
            return false;

        // -----------------------------------------------------
        // FIRST:
        // Make absolutely sure the planet knows whether it
        // currently belongs to a slot.
        // -----------------------------------------------------

        if (planet.CurrentSlot == null)
        {
            InitializePlanetIfAtSlot(planet);
        }

        // -----------------------------------------------------
        // Find ONLY an EMPTY slot.
        // -----------------------------------------------------

        PlanetHomeSlot targetSlot =
            FindNearestEmptySlot(
                planet.transform.position
            );

        if (targetSlot == null)
        {
            Debug.Log(
                $"{planet.name}: No empty slot found " +
                "within placement distance."
            );

            return false;
        }

        // -----------------------------------------------------
        // Extra safety.
        // -----------------------------------------------------

        if (!targetSlot.IsEmpty)
        {
            Debug.LogWarning(
                $"{targetSlot.name} is occupied. " +
                "Placement cancelled."
            );

            return false;
        }

        PlacePlanetInSlot(
            planet,
            targetSlot
        );

        return true;
    }

    // =========================================================
    // PLACE PLANET
    // =========================================================

    private void PlacePlanetInSlot(
        PlanetHomeSlot planet,
        PlanetHomeSlot slot)
    {
        if (planet == null || slot == null)
            return;

        // FINAL SAFETY CHECK.
        if (!slot.IsEmpty)
        {
            Debug.LogWarning(
                $"BLOCKED: {slot.name} already contains " +
                $"{slot.OccupyingPlanet?.name}"
            );

            return;
        }

        // Register relationship FIRST.
        planet.AssignToSlot(slot);

        // Position.
        planet.transform.position =
            slot.transform.position;

        // Rotation.
        planet.transform.rotation =
            slot.transform.rotation;

        // Restore THIS planet's own default scale.
        planet.transform.localScale =
            planet.GetDefaultPlanetScale();

        // Slot is occupied → hide sphere.
        slot.UpdateSlotVisual();

        // Hide information panel.
        PlanetInfoSelector infoSelector =
            planet.GetComponent<PlanetInfoSelector>();

        if (infoSelector != null)
        {
            infoSelector.PlanetReturnedToSlot();
        }
    }
}