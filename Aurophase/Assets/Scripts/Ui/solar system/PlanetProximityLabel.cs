using UnityEngine;
using TMPro;

public class PlanetProximityLabel : MonoBehaviour
{
    [Header("Planet")]
    [SerializeField] private string planetName = "EARTH";

    [Header("Label")]
    [SerializeField] private GameObject labelObject;
    [SerializeField] private TMP_Text labelText;

    [Header("Distance")]
    [SerializeField] private float showDistance = 1f;
    [SerializeField] private float hideDistance = 1.5f;

    [Header("Facing")]
    [SerializeField] private bool faceUser = true;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        if (labelText != null)
            labelText.text = planetName;

        if (labelObject != null)
            labelObject.SetActive(false);
    }

    private void Update()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;

            if (mainCamera == null)
                return;
        }

        float distance = Vector3.Distance(
            transform.position,
            mainCamera.transform.position
        );

        if (labelObject != null)
        {
            if (!labelObject.activeSelf && distance <= showDistance)
            {
                labelObject.SetActive(true);
            }
            else if (labelObject.activeSelf && distance >= hideDistance)
            {
                labelObject.SetActive(false);
            }
        }

        if (labelObject != null &&
            labelObject.activeSelf &&
            faceUser)
        {
            Vector3 direction =
                mainCamera.transform.position -
                labelObject.transform.position;

            if (direction.sqrMagnitude > 0.001f)
            {
                labelObject.transform.rotation =
                    Quaternion.LookRotation(
                        -direction,
                        Vector3.up
                    );
            }
        }
    }
}