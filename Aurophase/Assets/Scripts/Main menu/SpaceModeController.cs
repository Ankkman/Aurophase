using System.Collections;
using UnityEngine;

public class SpaceModeController : MonoBehaviour
{
    [Header("Modes")]
    [SerializeField] private GameObject solarSystemMode;
    [SerializeField] private GameObject planetMode;
    [SerializeField] private GameObject blackHoleMode;

    [Header("Mode Reset")]
    [SerializeField] private ResetModeTransform solarSystemReset;
    [SerializeField] private ResetModeTransform blackHoleReset;

    [Header("Planet Reset")]
    [SerializeField] private PlanetModeResetter planetModeResetter;

    [Header("Black Hole UI")]
    [SerializeField] private BlackHoleInfoController blackHoleInfoController;

    [Header("Passthrough")]
    [SerializeField] private AurophasePassthroughManager passthroughManager;

    private Coroutine modeTransitionCoroutine;


    [Header("Spatial Reference")]
    [SerializeField] private Transform mainUI;




    private void Start()
    {
        InitializeModeResets();
    }

    private void InitializeModeResets()
    {
        if (mainUI == null)
        {
            Debug.LogError(
                "SpaceModeController: MainUI is not assigned!"
            );

            return;
        }

        // -------------------------------------------------
        // SOLAR SYSTEM
        // -------------------------------------------------

        if (solarSystemReset != null)
        {
            solarSystemReset.Initialize(
                mainUI,
                solarSystemReset.transform
            );
        }

        // -------------------------------------------------
        // BLACK HOLE
        // -------------------------------------------------

        if (blackHoleReset != null)
        {
            blackHoleReset.Initialize(
                mainUI,
                blackHoleReset.transform
            );
        }

        // -------------------------------------------------
        // PLANETS
        // -------------------------------------------------

        if (planetModeResetter != null &&
            planetMode != null)
        {
            planetModeResetter.Initialize(
                mainUI,
                planetMode.transform
            );
        }

        Debug.Log(
            "Aurophase mode reset references initialized."
        );
    }

    // =====================================================
    // PUBLIC BUTTON METHODS
    // =====================================================

    public void ShowSolarSystem()
    {
        StartModeTransition(Mode.SolarSystem);
    }

    public void ShowPlanets()
    {
        StartModeTransition(Mode.Planets);
    }

    public void ShowBlackHole()
    {
        StartModeTransition(Mode.BlackHole);
    }

    // =====================================================
    // MODE ENUM
    // =====================================================

    private enum Mode
    {
        SolarSystem,
        Planets,
        BlackHole
    }

    // =====================================================
    // START TRANSITION
    // =====================================================

    private void StartModeTransition(Mode targetMode)
    {
        if (modeTransitionCoroutine != null)
        {
            StopCoroutine(modeTransitionCoroutine);
            modeTransitionCoroutine = null;
        }

        modeTransitionCoroutine =
            StartCoroutine(ChangeMode(targetMode));
    }

    // =====================================================
    // CHANGE MODE
    // =====================================================

    private IEnumerator ChangeMode(Mode targetMode)
    {
        bool goingToBlackHole =
            targetMode == Mode.BlackHole;

        bool currentlyBlackHole =
            blackHoleMode != null &&
            blackHoleMode.activeSelf;

        Debug.Log(
            "Changing mode to: " + targetMode
        );

        // -------------------------------------------------
        // HIDE CURRENT CONTENT
        // -------------------------------------------------

        DisableAllModes();

        // =================================================
        // BLACK HOLE
        // =================================================

        if (goingToBlackHole)
        {
            Debug.Log(
                "Preparing BLACK HOLE..."
            );

            // -------------------------------------------------
            // ENVIRONMENT
            // -------------------------------------------------

            if (passthroughManager != null)
            {
                passthroughManager.SetBlackHoleMode();

                yield return new WaitUntil(
                    () => !passthroughManager.IsTransitioning
                );
            }

            // -------------------------------------------------
            // ACTIVATE FIRST
            // -------------------------------------------------

            if (blackHoleMode != null)
            {
                blackHoleMode.SetActive(true);
            }

            // -------------------------------------------------
            // WAIT ONE FRAME
            // This allows ResetModeTransform.Start()
            // to initialize using the CURRENT MainUI position.
            // -------------------------------------------------

            yield return null;

            // -------------------------------------------------
            // NOW RESET RELATIVE TO CURRENT MAIN UI
            // -------------------------------------------------

            if (blackHoleReset != null)
            {
                blackHoleReset.ResetToDefault();
            }

            Debug.Log(
                "BLACK HOLE MODE ACTIVATED."
            );
        }

        // =================================================
        // NORMAL MODES
        // =================================================

        else
        {
            // -------------------------------------------------
            // LEAVING BLACK HOLE
            // -------------------------------------------------

            if (currentlyBlackHole)
            {
                Debug.Log(
                    "Leaving BLACK HOLE..."
                );

                if (passthroughManager != null)
                {
                    passthroughManager.SetNormalMode();

                    yield return new WaitUntil(
                        () => !passthroughManager.IsTransitioning
                    );
                }
            }
            else
            {
                // Already in normal environment.
                if (passthroughManager != null)
                {
                    passthroughManager.SetNormalMode();
                }
            }

            // =================================================
            // SOLAR SYSTEM
            // =================================================

            if (targetMode == Mode.SolarSystem)
            {
                Debug.Log(
                    "Preparing SOLAR SYSTEM..."
                );

                // Activate first.
                if (solarSystemMode != null)
                {
                    solarSystemMode.SetActive(true);
                }

                // Allow ResetModeTransform.Start()
                // to initialize.
                yield return null;

                // Reset relative to CURRENT MainUI position.
                if (solarSystemReset != null)
                {
                    solarSystemReset.ResetToDefault();
                }

                Debug.Log(
                    "SOLAR SYSTEM MODE ACTIVATED."
                );
            }

            // =================================================
            // PLANETS
            // =================================================

            else if (targetMode == Mode.Planets)
            {
                Debug.Log(
                    "Preparing PLANETS..."
                );

                // Activate first.
                if (planetMode != null)
                {
                    planetMode.SetActive(true);
                }

                // Allow PlanetMode and its children
                // to initialize.
                yield return null;

                // Reset relative to CURRENT MainUI position.
                if (planetModeResetter != null)
                {
                    planetModeResetter.ResetAllPlanets();
                }

                Debug.Log(
                    "PLANET MODE ACTIVATED."
                );
            }
        }

        modeTransitionCoroutine = null;
    }

    // =====================================================
    // DISABLE ALL MODES
    // =====================================================

    private void DisableAllModes()
    {
        if (solarSystemMode != null)
        {
            solarSystemMode.SetActive(false);
        }

        if (planetMode != null)
        {
            planetMode.SetActive(false);
        }

        if (blackHoleMode != null)
        {
            blackHoleMode.SetActive(false);
        }

        if (blackHoleInfoController != null)
        {
            blackHoleInfoController.HideInfo();
        }
    }
}