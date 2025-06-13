using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Menus")]
    public GameObject FirstMenuPanel;         // The main start menu
    public GameObject SecondMenuPanel;        // The tools menu or next stage menu

    [Header("Game Area (Optional)")]
    public GameObject gameArea;               // Optional game content to activate on start

    // Called when "Start New" is clicked
    public void GoToSecondMenu()
    {
        Debug.Log("Navigating to second menu...");

        if (FirstMenuPanel != null)
            FirstMenuPanel.SetActive(false);

        if (SecondMenuPanel != null)
            SecondMenuPanel.SetActive(true);

        if (gameArea != null)
            gameArea.SetActive(true);

        Debug.Log("Switched to second menu.");
    }

    // Placeholder for loading old game
    public void LoadOldGame()
    {
        Debug.Log("Load Old Game logic goes here...");
    }

    // Placeholder for settings panel
    public void OpenSettings()
    {
        Debug.Log("Open Settings logic goes here...");
    }

    // Exit button function
    public void ExitGame()
    {
        Debug.Log("Exiting the game...");
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;  // Stop Play mode in Editor
#else
    Application.Quit();  // Quit app when built
#endif
    }
}
