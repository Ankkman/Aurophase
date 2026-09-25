using UnityEngine;

public class PlanetModeController : MonoBehaviour
{
    [Header("Planet Mode")]
    [SerializeField] private GameObject planetMode;

    public void OpenPlanets()
    {
        if (planetMode == null)
        {
            Debug.LogError("PlanetModeController: Planet Mode is not assigned.");
            return;
        }

        planetMode.SetActive(true);
    }

    public void ClosePlanets()
    {
        if (planetMode == null)
            return;

        planetMode.SetActive(false);
    }
}