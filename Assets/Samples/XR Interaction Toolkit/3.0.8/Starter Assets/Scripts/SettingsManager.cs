using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public GameObject settingsPanel;
    public Slider volumeSlider;
    public AudioSource audioSource; // The AudioSource that plays your game audio

    void Start()
    {
        // Set slider value from saved value or default
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
        volumeSlider.value = savedVolume;
        ApplyVolume(savedVolume);
        
        // Listen to volume change
        volumeSlider.onValueChanged.AddListener(ApplyVolume);
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void ApplyVolume(float value)
    {
        AudioListener.volume = value;
        if (audioSource != null)
            audioSource.volume = value;

        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.Save();
    }
}
