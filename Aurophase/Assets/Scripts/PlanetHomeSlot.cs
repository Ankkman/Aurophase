using System.Collections;
using UnityEngine;

public class PlanetHomeSlot : MonoBehaviour
{
    [Header("Object Type")]
    [SerializeField] private bool isSlotObject = false;

    [Header("Slot Placement")]
    [SerializeField] private float placementDistance = 0.35f;

    [Header("Slot Visual")]
    [SerializeField] private GameObject emptySlotIndicator;

    [Header("Information Panel")]
    [SerializeField] private PlanetInfoSelector infoSelector;

    private PlanetHomeSlot currentSlot;
    private PlanetHomeSlot occupyingPlanet;

    private Vector3 defaultPlanetScale;
    private Quaternion defaultPlanetRotation;


    [Header("Original Home")]
    [SerializeField] private PlanetHomeSlot originalHomeSlot;


    // =========================================================
    // PROPERTIES
    // =========================================================

    public bool IsEmpty
    {
        get
        {
            return occupyingPlanet == null;
        }
    }

    public PlanetHomeSlot OccupyingPlanet
    {
        get
        {
            return occupyingPlanet;
        }
    }

    public PlanetHomeSlot CurrentSlot
    {
        get
        {
            return currentSlot;
        }
    }


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        if (!isSlotObject && infoSelector == null)
        {
            infoSelector =
                GetComponent<PlanetInfoSelector>();
        }

        // Planet defaults are stored in Start().
    }


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        if (isSlotObject)
        {
            if (PlanetSlotManager.Instance != null)
            {
                PlanetSlotManager.Instance.RegisterSlot(this);
            }

            UpdateSlotVisual();
        }

        defaultPlanetScale = transform.localScale;
        defaultPlanetRotation = transform.localRotation;
    }


    public void SetOriginalHomeSlot(PlanetHomeSlot slot)
    {
        originalHomeSlot = slot;
    }

    public void ResetToOriginalHome()
    {
        if (isSlotObject)
            return;

        if (originalHomeSlot == null)
        {
            Debug.LogWarning(
                $"{name}: Original home slot has not been assigned."
            );
            return;
        }

        // Make sure the planet is removed from any current slot.
        DetachFromCurrentSlot();

        // Restore original transform.
        transform.position = originalHomeSlot.transform.position;
        transform.rotation = originalHomeSlot.transform.rotation;
        transform.localScale = defaultPlanetScale;

        // Assign back to original slot.
        AssignToSlot(originalHomeSlot);

        // Restore the planet's original rotation explicitly.
        transform.rotation = originalHomeSlot.transform.rotation;

        // Restore the planet's original scale.
        transform.localScale = defaultPlanetScale;

        // Hide the empty-slot indicator.
        originalHomeSlot.UpdateSlotVisual();

        Debug.Log(
            $"{name}: Reset to original home slot " +
            $"{originalHomeSlot.name}"
        );
    }


    // =========================================================
    // ON ENABLE
    // =========================================================

    private void OnEnable()
    {
        if (isSlotObject)
            return;

        StartCoroutine(InitializePlanetNextFrame());
    }


    private IEnumerator InitializePlanetNextFrame()
    {
        // Wait until all objects that became active together
        // have completed their activation.
        yield return null;

        if (PlanetSlotManager.Instance != null)
        {
            PlanetSlotManager.Instance
                .InitializePlanetIfAtSlot(this);
        }
    }


    // =========================================================
    // DESTROY
    // =========================================================

    private void OnDestroy()
    {
        if (isSlotObject &&
            PlanetSlotManager.Instance != null)
        {
            PlanetSlotManager.Instance
                .UnregisterSlot(this);
        }
    }


    // =========================================================
    // GRAB
    // =========================================================

    public void OnGrab()
    {
        if (isSlotObject)
            return;

        // Was this planet actually sitting inside a slot?
        bool wasInSlot = currentSlot != null;

        // Free the slot immediately.
        DetachFromCurrentSlot();

        // -----------------------------------------------------
        // SOUND
        // -----------------------------------------------------

        if (AurophaseAudioManager.Instance != null)
        {
            if (wasInSlot)
            {
                // Special sound:
                // Planet taken out of its home/rack slot.
                AurophaseAudioManager.Instance
                    .PlayPlanetSlotGrab();
            }
            else
            {
                // Normal free-space grab.
                AurophaseAudioManager.Instance
                    .PlayGrab();
            }
        }

        // -----------------------------------------------------
        // INFO PANEL
        // -----------------------------------------------------

        if (infoSelector != null)
        {
            infoSelector.SelectPlanet();
        }
    }


    // =========================================================
    // RELEASE
    // =========================================================

    public void OnRelease()
    {
        if (isSlotObject)
            return;

        if (PlanetSlotManager.Instance == null)
        {
            Debug.LogWarning("PlanetSlotManager not found.");
            return;
        }

        // Try to place the planet into a slot.
        PlanetSlotManager.Instance.TryPlacePlanet(this);

        // -----------------------------------------------------
        // CHECK ACTUAL RESULT
        // -----------------------------------------------------

        bool isNowInSlot = CurrentSlot != null;

        if (AurophaseAudioManager.Instance != null)
        {
            if (isNowInSlot)
            {
                // Successfully placed into a slot.
                AurophaseAudioManager.Instance
                    .PlayPlanetSlotDrop();
            }
            else
            {
                // Released somewhere in free space.
                AurophaseAudioManager.Instance
                    .PlayDrop();
            }
        }
    }

    // =========================================================
    // DETACH FROM SLOT
    // =========================================================

    public void DetachFromCurrentSlot()
    {
        if (currentSlot == null)
            return;

        if (currentSlot.occupyingPlanet == this)
        {
            currentSlot.occupyingPlanet = null;

            // SLOT IS NOW EMPTY.
            // Sphere appears immediately.
            currentSlot.UpdateSlotVisual();
        }

        currentSlot = null;
    }


    // =========================================================
    // ASSIGN TO SLOT
    // =========================================================

    public void AssignToSlot(
        PlanetHomeSlot newSlot)
    {
        if (newSlot == null)
            return;

        // Never allow two planets in one slot.
        if (newSlot.occupyingPlanet != null &&
            newSlot.occupyingPlanet != this)
        {
            Debug.LogWarning(
                $"{newSlot.name} is already occupied."
            );

            return;
        }

        // Remove from previous slot.
        if (currentSlot != null &&
            currentSlot != newSlot)
        {
            if (currentSlot.occupyingPlanet == this)
            {
                currentSlot.occupyingPlanet = null;
                currentSlot.UpdateSlotVisual();
            }
        }

        currentSlot = newSlot;

        newSlot.occupyingPlanet = this;

        // Occupied → sphere OFF.
        newSlot.UpdateSlotVisual();

        // Planet successfully returned to a slot.
        if (infoSelector != null)
        {
            infoSelector.PlanetReturnedToSlot();
        }

    }


    // =========================================================
    // DEFAULT SCALE
    // =========================================================

    public Vector3 GetDefaultPlanetScale()
    {
        return defaultPlanetScale;
    }


    // =========================================================
    // DEFAULT ROTATION
    // =========================================================

    public Quaternion GetDefaultPlanetRotation()
    {
        return defaultPlanetRotation;
    }


    // =========================================================
    // IS PLANET
    // =========================================================

    public bool IsPlanet()
    {
        return !isSlotObject;
    }


    // =========================================================
    // SLOT VISUAL
    // =========================================================

    public void UpdateSlotVisual()
    {
        if (!isSlotObject)
            return;

        if (emptySlotIndicator == null)
            return;

        // ONLY EMPTY SLOTS SHOW THEIR SPHERE.
        emptySlotIndicator.SetActive(IsEmpty);
    }
}