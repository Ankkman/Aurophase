using UnityEngine;

public class OrbitAroundTrack : MonoBehaviour
{
    [Header("Orbit Track")]
    [Tooltip("Drag the corresponding Orbit object here (e.g., Object_40 for Earth)")]
    public Transform orbitTrack;

    [Header("Orbit Speed")]
    [Tooltip("Speed of the orbit revolution")]
    public float orbitSpeed = 20f;

    void Update()
    {
        if (orbitTrack != null)
        {
            // Revolves this planet around the center of its specific orbit track
            // It uses the orbit's 'up' direction so it stays perfectly aligned even if the orbit is tilted!
            transform.RotateAround(orbitTrack.position, orbitTrack.up, orbitSpeed * Time.deltaTime);
        }
    }
}
