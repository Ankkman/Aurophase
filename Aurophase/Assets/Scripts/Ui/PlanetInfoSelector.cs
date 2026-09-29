using UnityEngine;

public class PlanetInfoSelector : MonoBehaviour
{
    private PlanetInfoData infoData;

    private void Awake()
    {
        infoData = GetComponent<PlanetInfoData>();
    }

    public void SelectPlanet()
    {
        if (PlanetInfoManager.Instance == null)
        {
            Debug.LogWarning("PlanetInfoManager not found.");
            return;
        }

        if (infoData == null)
        {
            Debug.LogWarning(
                gameObject.name + " does not have PlanetInfoData."
            );
            return;
        }

        PlanetInfoManager.Instance.ShowPlanetInfo(infoData);
    }

    // Called when this planet successfully returns to ANY slot.
    public void PlanetReturnedToSlot()
    {
        if (PlanetInfoManager.Instance == null)
            return;

        PlanetInfoManager.Instance.HidePlanetInfoForPlanet(infoData);
    }
}