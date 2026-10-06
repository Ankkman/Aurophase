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

    [Header("Black Hole UI")]
    [SerializeField] private BlackHoleInfoController blackHoleInfoController;

    [Header("Passthrough")]
    [SerializeField] private AurophasePassthroughManager passthroughManager;

    private Coroutine modeTransitionCoroutine;

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

        modeTransitionCoroutine = StartCoroutine(
            ChangeMode(targetMode)
        );
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
        // HIDE EVERYTHING
        // -------------------------------------------------

        DisableAllModes();

        // =================================================
        // BLACK HOLE
        // =================================================

        if (goingToBlackHole)
        {
            Debug.Log("Preparing BLACK HOLE...");

            // Reset while hidden.
            if (blackHoleReset != null)
            {
                blackHoleReset.ResetToDefault();

                Debug.Log(
                    "Black Hole transform reset."
                );
            }

            // Change environment.
            if (passthroughManager != null)
            {
                passthroughManager.SetBlackHoleMode();

                yield return new WaitUntil(
                    () => !passthroughManager.IsTransitioning
                );
            }

            // Activate mode.
            if (blackHoleMode != null)
            {
                blackHoleMode.SetActive(true);
            }

            // Reset AGAIN after activation.
            // This guarantees the model is placed correctly
            // even if another component changed it.
            if (blackHoleReset != null)
            {
                blackHoleReset.ResetToDefault();
            }

            Debug.Log(
                "BLACK HOLE MODE ACTIVATED."
            );
        }

        // =================================================
        // NORMAL ENVIRONMENT
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

                // Reset before activation.
                if (solarSystemReset != null)
                {
                    solarSystemReset.ResetToDefault();

                    Debug.Log(
                        "Solar System transform reset."
                    );
                }

                // Activate.
                if (solarSystemMode != null)
                {
                    solarSystemMode.SetActive(true);
                }

                // Reset AGAIN after activation.
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
                if (planetMode != null)
                {
                    planetMode.SetActive(true);
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