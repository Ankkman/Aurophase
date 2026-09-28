using UnityEngine;

public class PlanetModeController : MonoBehaviour
{
    [Header("Planet Mode")]
    [SerializeField] private GameObject planetMode;

    private void Awake()
    {
        if (planetMode == null)
        {
            Debug.LogError(
                "PlanetModeController: Planet Mode reference is NOT assigned."
            );
            return;
        }

        planetMode.SetActive(false);
    }

    public void ShowPlanets()
    {
        Debug.Log("PLANETS BUTTON PRESSED");

        if (planetMode == null)
        {
            Debug.LogError(
                "PlanetModeController: Planet Mode reference is missing."
            );
            return;
        }

        planetMode.SetActive(true);

        Debug.Log("Planet Mode ACTIVATED");
    }
}