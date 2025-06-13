using UnityEngine;
using System.IO;

[System.Serializable]
public class SaveData
{
    public string selectedShape;
    public Vector3 position;
    public Vector3 scale;
    public Quaternion rotation;

    public bool inRoomMenuActive;
    public bool configMenuActive;
}

public class SaveManager : MonoBehaviour
{
    public GameObject carvedObject;       // Assign in Inspector
    public string selectedShape = "Cube"; // Set from dropdown logic
    public GameObject inRoomMenuPanel;    // Assign in Inspector
    public GameObject configMenuPanel;    // Assign in Inspector

    private string savePath;

    void Start()
    {
        savePath = Application.persistentDataPath + "/savefile.json";
    }

    public void SaveScene()
    {
        SaveData data = new SaveData
        {
            selectedShape = selectedShape,
            position = carvedObject.transform.position,
            scale = carvedObject.transform.localScale,
            rotation = carvedObject.transform.rotation,
            inRoomMenuActive = inRoomMenuPanel.activeSelf,
            configMenuActive = configMenuPanel.activeSelf
        };

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(savePath, json);
        Debug.Log("Saved to: " + savePath);
    }

    public void LoadScene()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("No save file found.");
            return;
        }

        string json = File.ReadAllText(savePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        selectedShape = data.selectedShape;
        carvedObject.transform.position = data.position;
        carvedObject.transform.localScale = data.scale;
        carvedObject.transform.rotation = data.rotation;
        inRoomMenuPanel.SetActive(data.inRoomMenuActive);
        configMenuPanel.SetActive(data.configMenuActive);

        Debug.Log("Loaded from: " + savePath);
    }
}
