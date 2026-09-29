using UnityEngine;

public class QuitApp : MonoBehaviour
{
    public void Quit()
    {
        Debug.Log("Quit App pressed.");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_ANDROID
        Application.Quit();
#else
        Application.Quit();
#endif
    }
}