using System.Collections;
using UnityEngine;

public class SpaceModeController : MonoBehaviour
{
    [Header("Modes")]
    [SerializeField] private GameObject solarSystemMode;
    [SerializeField] private GameObject planetMode;
    [SerializeField] private GameObject blackHoleMode;

    [Header("Black Hole UI")]
    [SerializeField] private BlackHoleInfoController blackHoleInfoController;

    [Header("Passthrough")]
    [SerializeField] private AurophasePassthroughManager passthroughManager;

    private Coroutine modeTransitionCoroutine;

    // =====================================================
    // SOLAR SYSTEM
    // =====================================================

    public void ShowSolarSystem()
    {
        StartModeTransition(Mode.SolarSystem);
    }

    // =====================================================
    // PLANETS
    // =====================================================

    public void ShowPlanets()
    {
        StartModeTransition(Mode.Planets);
    }

    // =====================================================
    // BLACK HOLE
    // =====================================================

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
    // START MODE TRANSITION
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

        // -------------------------------------------------
        // HIDE CURRENT CONTENT FIRST
        // -------------------------------------------------

        DisableAllModes();

        // -------------------------------------------------
        // ENTER BLACK HOLE
        // -------------------------------------------------

        if (goingToBlackHole)
        {
            Debug.Log("Preparing BLACK HOLE environment...");

            if (passthroughManager != null)
            {
                passthroughManager.SetBlackHoleMode();

                // Wait until environment becomes dark.
                yield return new WaitUntil(
                    () => !passthroughManager.IsTransitioning
                );
            }

            // Now show Black Hole.
            if (blackHoleMode != null)
            {
                blackHoleMode.SetActive(true);
            }

            Debug.Log("BLACK HOLE MODE ACTIVATED.");
        }

        // -------------------------------------------------
        // RETURN FROM BLACK HOLE
        // -------------------------------------------------

        else
        {
            if (currentlyBlackHole)
            {
                Debug.Log(
                    "Leaving BLACK HOLE - restoring environment..."
                );

                if (passthroughManager != null)
                {
                    passthroughManager.SetNormalMode();

                    // Wait until normal environment is restored.
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

            // -------------------------------------------------
            // SHOW TARGET NORMAL MODE
            // -------------------------------------------------

            if (targetMode == Mode.SolarSystem)
            {
                if (solarSystemMode != null)
                {
                    solarSystemMode.SetActive(true);
                }

                Debug.Log(
                    "SOLAR SYSTEM MODE ACTIVATED."
                );
            }
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

        // Always hide independent Black Hole information UI.
        if (blackHoleInfoController != null)
        {
            blackHoleInfoController.HideInfo();
        }
    }
}