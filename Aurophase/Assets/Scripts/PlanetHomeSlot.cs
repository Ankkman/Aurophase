using UnityEngine;

public class PlanetHomeSlot : MonoBehaviour
{
    [Header("Home Slot")]
    [SerializeField] private Transform homeSlot;

    [Header("Return Settings")]
    [SerializeField] private float returnDistance = 0.35f;

    private bool isBeingGrabbed;

    public void OnGrab()
    {
        isBeingGrabbed = true;
    }

    public void OnRelease()
    {
        isBeingGrabbed = false;
        TryReturnHome();
    }

    private void TryReturnHome()
    {
        if (homeSlot == null)
        {
            Debug.LogWarning($"{name}: Home Slot is not assigned.");
            return;
        }

        float distance = Vector3.Distance(
            transform.position,
            homeSlot.position
        );

        if (distance <= returnDistance)
        {
            ReturnHome();
        }
    }

    private void ReturnHome()
    {
        transform.position = homeSlot.position;
        transform.rotation = homeSlot.rotation;
        transform.localScale = homeSlot.localScale;
    }
}