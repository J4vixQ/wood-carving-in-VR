using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SecondMenuController : MonoBehaviour
{
    [Header("Menus")]
    public GameObject SecondMenuPanel;      // The current menu (tools/options)
    public GameObject FirstMenuPanel;       // Optional: go back to main menu

    [Header("Tool & Save Area (Optional)")]
    public GameObject ToolArea;             // Area with tools or content
    public GameObject SaveMessage;          // Optional popup/feedback

    // Called when "Tool" is clicked
    public void OnTool()
    {
        Debug.Log("Tool mode activated.");

        if (ToolArea != null)
            ToolArea.SetActive(true);

        if (SecondMenuPanel != null)
            SecondMenuPanel.SetActive(false);
    }

    // Called when "Save" is clicked
    public void OnSave()
    {
        Debug.Log("Game saved.");
        PlayerPrefs.SetString("lastScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();

        if (SaveMessage != null)
            StartCoroutine(ShowSaveMessage());
    }



    // Called when "Settings" is clicked
    public void OpenSettings()
    {
        Debug.Log("Settings popup from second menu.");
        // You can show a settings panel or open another UI
    }

    // Called when "Exit" is clicked
    public void ExitGame()
    {
        Debug.Log("Exiting the game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Optional: Go back to the first menu
    public void GoBackToMain()
    {
        if (SecondMenuPanel != null)
            SecondMenuPanel.SetActive(false);

        if (FirstMenuPanel != null)
            FirstMenuPanel.SetActive(true);
    }
    private IEnumerator ShowSaveMessage()
    {
        SaveMessage.SetActive(true);     // Show the message
        yield return new WaitForSeconds(2f); // Wait for 2 seconds (or change duration)
        SaveMessage.SetActive(false);    // Hide the message
    }
}


