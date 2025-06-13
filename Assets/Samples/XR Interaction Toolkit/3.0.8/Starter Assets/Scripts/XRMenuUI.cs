// using UnityEngine;
// using UnityEngine.UI;

// public class XRMenuUI : MonoBehaviour
// {
//     [Header("Menu Elements")]
//     public Button startButton;
//     public Button carvingToolButton;
//     public Button saveButton;
//     public Button tutorialButton;
//     public Button SettingsButton;
//     public Button exitButton;

//     void Start()
//     {
//     //     startButton.onClick.AddListener(OnStart);
//     //     carvingToolButton.onClick.AddListener(OnCarvingTool);
//     //     shedButton.onClick.AddListener(OnShed);
//     //     tutorialButton.onClick.AddListener(OnTutorial);
//     //     settingsButton.onClick.AddListener(OnSettings);
//     //     exitButton.onClick.AddListener(OnExit);
//     // }

//     void OnStart()
//     {
//         Debug.Log("Start option selected.");
//     }

//     void OnCarvingTool()
//     {
//         Debug.Log("Carving Tool option selected.");
//     }

//     void OnSave()
//     {
//         Debug.Log("Shed option selected.");
//     }

//     void OnTutorial()
//     {
//         Debug.Log("Tutorial option selected.");
//     }

//     void OnSettings()
//     {
//         Debug.Log("Settings option selected.");
//     }

//     void OnExit()
//     {
//         Debug.Log("Exit selected. Application will quit.");
// #if UNITY_EDITOR
//         UnityEditor.EditorApplication.isPlaying = false;
// #else
//         Application.Quit();
// #endif
//     }
// }
