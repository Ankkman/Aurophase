using UnityEngine;

public class PlanetModeController : MonoBehaviour
{
    [Header("Planet Mode")]
    [SerializeField] private GameObject planetMode;

    [Header("Other Modes")]
    [SerializeField] private BlackHoleInfoController blackHoleInfoController;

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

        // Hide Black Hole information panel
        if (blackHoleInfoController != null)
        {
            blackHoleInfoController.HideInfo();
        }

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