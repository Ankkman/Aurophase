using UnityEngine;
using UnityEngine.UI;

public class MenuDockManager : MonoBehaviour
{
    [Header("Menu Buttons")]
    public Button btnSolarSystem;
    public Button btnPlanets;
    public Button btnBlackHole;

    void Start()
    {
        // Attach click events to the buttons
        if (btnSolarSystem != null)
            btnSolarSystem.onClick.AddListener(OnSolarSystemClicked);
            
        if (btnPlanets != null)
            btnPlanets.onClick.AddListener(OnPlanetsClicked);
            
        if (btnBlackHole != null)
            btnBlackHole.onClick.AddListener(OnBlackHoleClicked);
    }

    private void OnSolarSystemClicked()
    {
        Debug.Log("Solar System Mode Triggered - (We will build this in Phase 6)");
        // Logic to clear scene and spawn full solar system will go here
    }

    private void OnPlanetsClicked()
    {
        Debug.Log("Planets Mode Triggered - (We will build this next in Phase 4)");
        // Logic to spawn the horizontal planet tray will go here
    }

    private void OnBlackHoleClicked()
    {
        Debug.Log("Black Hole Mode Triggered - (We will build this in Phase 5)");
        // Logic to fade passthrough to black and spawn Black Hole will go here
    }
}