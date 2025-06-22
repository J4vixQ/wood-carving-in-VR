using UnityEngine;
using UnityEngine.SceneManagement;

public static class SaveManager
{
    public static void SaveGame(Transform player)
    {
        PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);

        if (player != null)
        {
            PlayerPrefs.SetFloat("PlayerPosX", player.position.x);
            PlayerPrefs.SetFloat("PlayerPosY", player.position.y);
            PlayerPrefs.SetFloat("PlayerPosZ", player.position.z);
        }

        PlayerPrefs.Save();
        Debug.Log("Game saved by SaveManager.");
    }

    public static void LoadGame(Transform player)
    {
        if (!PlayerPrefs.HasKey("LastScene"))
        {
            Debug.Log("No saved game found.");
            return;
        }

        string sceneName = PlayerPrefs.GetString("LastScene");
        SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) =>
        {
            SceneManager.sceneLoaded -= null; // optional cleanup

            if (player != null)
            {
                float x = PlayerPrefs.GetFloat("PlayerPosX", player.position.x);
                float y = PlayerPrefs.GetFloat("PlayerPosY", player.position.y);
                float z = PlayerPrefs.GetFloat("PlayerPosZ", player.position.z);
                player.position = new Vector3(x, y, z);
            }

            Debug.Log("Game loaded by SaveManager.");
        };

        SceneManager.LoadScene(sceneName);
    }
}
