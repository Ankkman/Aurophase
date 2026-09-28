using UnityEngine;

public class PlanetInfoData : MonoBehaviour
{
    [Header("Basic Information")]
    public string planetName;

    [TextArea(3, 6)]
    public string description;

    [Header("Statistics")]
    public string diameter;
    public string orbitalPeriod;
    public string distanceFromSun;
}