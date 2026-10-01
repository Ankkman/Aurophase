using UnityEngine;

public class BlackHoleInfoController : MonoBehaviour
{
    [Header("Black Hole")]
    [SerializeField] private Transform blackHole;

    [Header("Info Canvas")]
    [SerializeField] private GameObject infoCanvas;
    [SerializeField] private Transform infoCanvasTransform;

    [Header("Canvas Position")]
    [SerializeField] private float horizontalOffset = 1.0f;
    [SerializeField] private float verticalOffset = 0.2f;
    [SerializeField] private float depthOffset = 0f;

    [Header("Canvas Facing")]
    [SerializeField] private bool alwaysFaceUser = true;
    [SerializeField] private bool flipPanel = false;

    private Camera mainCamera;

    private void Awake()
    {
        if (blackHole == null)
            blackHole = transform;

        mainCamera = Camera.main;

        // Start hidden
        if (infoCanvas != null)
            infoCanvas.SetActive(false);
    }

    private void LateUpdate()
    {
        if (infoCanvas == null || !infoCanvas.activeSelf)
            return;

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
            return;

        UpdateCanvasPosition();
        UpdateCanvasRotation();
    }

    // =====================================================
    // BLACK HOLE GRAB
    // =====================================================

    public void OnGrab()
    {
        ShowInfo();

        if (AurophaseAudioManager.Instance != null)
        {
            AurophaseAudioManager.Instance.PlayGrab();
        }
    }

    // =====================================================
    // BLACK HOLE RELEASE
    // =====================================================

    public void OnRelease()
    {
        if (AurophaseAudioManager.Instance != null)
        {
            AurophaseAudioManager.Instance.PlayDrop();
        }
    }

    // =====================================================
    // SHOW
    // =====================================================

    public void ShowInfo()
    {
        if (infoCanvas == null)
            return;

        infoCanvas.SetActive(true);

        UpdateCanvasPosition();
        UpdateCanvasRotation();
    }

    // =====================================================
    // HIDE
    // =====================================================

    public void HideInfo()
    {
        if (infoCanvas == null)
            return;

        infoCanvas.SetActive(false);
    }

    // =====================================================
    // POSITION
    // =====================================================

    private void UpdateCanvasPosition()
    {
        if (blackHole == null || infoCanvasTransform == null)
            return;

        if (mainCamera == null)
            return;

        Vector3 cameraRight = mainCamera.transform.right;
        Vector3 cameraUp = mainCamera.transform.up;
        Vector3 cameraForward = mainCamera.transform.forward;

        Vector3 targetPosition =
            blackHole.position
            + cameraRight * horizontalOffset
            + cameraUp * verticalOffset
            + cameraForward * depthOffset;

        infoCanvasTransform.position = targetPosition;
    }

    // =====================================================
    // ROTATION
    // =====================================================

    private void UpdateCanvasRotation()
    {
        if (!alwaysFaceUser)
            return;

        if (mainCamera == null || infoCanvasTransform == null)
            return;

        Vector3 direction =
            mainCamera.transform.position
            - infoCanvasTransform.position;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(
                direction.normalized,
                Vector3.up
            );

        if (flipPanel)
        {
            targetRotation *=
                Quaternion.Euler(0f, 180f, 0f);
        }

        infoCanvasTransform.rotation = targetRotation;
    }
}