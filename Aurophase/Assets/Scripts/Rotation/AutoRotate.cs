using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("The base speed for this specific planet")]
    public Vector3 rotationSpeed = new Vector3(0f, 10f, 0f);

    [Header("Global Control")]
    [Tooltip("Change this slider to speed up or slow down this motion! (1 is normal, 0.5 is half speed)")]
    [Range(0f, 2f)]
    public float speedMultiplier = 1.0f; 

    void Update()
    {
        // Applies the individual speed multiplied by your inspector setting
        transform.Rotate(rotationSpeed * speedMultiplier * Time.deltaTime, Space.Self);
    }
}
