using UnityEngine;

public class SpaceModeController : MonoBehaviour
{
    [Header("Modes")]
    [SerializeField] private GameObject solarSystemMode;
    [SerializeField] private GameObject planetMode;
    [SerializeField] private GameObject blackHoleMode;

    public void ShowSolarSystem()
    {
        DisableAllModes();

        if (solarSystemMode != null)
            solarSystemMode.SetActive(true);

        Debug.Log("SOLAR SYSTEM MODE ACTIVATED");
    }

    public void ShowPlanets()
    {
        DisableAllModes();

        if (planetMode != null)
            planetMode.SetActive(true);

        Debug.Log("PLANET MODE ACTIVATED");
    }

    public void ShowBlackHole()
    {
        DisableAllModes();

        if (blackHoleMode != null)
            blackHoleMode.SetActive(true);

        Debug.Log("BLACK HOLE MODE ACTIVATED");
    }

    private void DisableAllModes()
    {
        if (solarSystemMode != null)
            solarSystemMode.SetActive(false);

        if (planetMode != null)
            planetMode.SetActive(false);

        if (blackHoleMode != null)
            blackHoleMode.SetActive(false);
    }
}