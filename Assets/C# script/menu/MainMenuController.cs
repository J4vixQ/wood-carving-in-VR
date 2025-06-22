using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Menus")]
    public GameObject FirstMenuPanel;         // The main start menu
    public GameObject SecondMenuPanel;        // The tools menu or next stage menu

    [Header("Game Area (Optional)")]
    public GameObject gameArea;               // Optional game content to activate on start

    [Header("Player Reference")]
    public Transform playerTransform;         // Player object reference to save/load position

    [Header("XR Controller Menu Button")]
    [SerializeField]
    InputActionReference m_MenuButtonAction;  // assign this to your “MenuPress” action in the Inspector

    public GameObject _carving_object; 

    // Enable input action
    void OnEnable()
    {
        var action = m_MenuButtonAction.action;
        action.Enable();
        action.performed += OnMenuButtonPressed;
    }

    void OnDisable()
    {
        var action = m_MenuButtonAction.action;
        action.performed -= OnMenuButtonPressed;
        action.Disable();
    }

    // Triggered when XR menu button is pressed
    void OnMenuButtonPressed(InputAction.CallbackContext ctx)
    {
        Debug.Log("XR Menu Button Pressed");
        // You can trigger UI toggle or save here if desired
    }

    // Start a brand new game
    public void StartNewGame()
    {
        
        
        Debug.Log("Start New Game");
        //SaveManager.LoadGame(playerTransform);
        CarvingObject carvingObject = _carving_object.GetComponent<CarvingObject>();
        bool r = carvingObject.loadFromFile("default");
        if (r)
        {
            Debug.Log("Load successful");
        }
        else
        {
            Debug.LogError("Load failed");
        }

    }

    // Load a previously saved game
    public void LoadOldGame()
    {
        //SaveManager.LoadGame(playerTransform);
        CarvingObject carvingObject = _carving_object.GetComponent<CarvingObject>();
        bool r = carvingObject.loadFromFile("Test");  // save_20250622_1
        if (r)
        {
            Debug.Log("Load successful");
        }
        else
        {
            Debug.LogError("Load failed");
        }
    }

    // Save the game using the SaveManager
    public void SaveGame()
    {
        CarvingObject carvingObject = _carving_object.GetComponent<CarvingObject>();
        bool r = carvingObject.saveToFile("Test");
        if (r)
        {
            Debug.Log("Save successful");
        }
        else
        {
            Debug.LogError("Save failed");
        }
        //SaveManager.SaveGame(playerTransform);
    }

    // Settings placeholder
    public void OpenSettings()
    {
        Debug.Log("Open Settings logic goes here...");
    }

    // Quit the application or stop the editor
    public void ExitGame()
    {
        Debug.Log("Exiting the game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
