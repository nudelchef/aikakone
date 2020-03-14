using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using SimpleJSON;
using static audioManager;

public class SettingsController : MonoBehaviour {

    string pathToSettingJson = "";
    JSONNode settings;

    public Toggle fullscreenToggle;
    public Slider masterVolume;
    public Slider musicVolume;
    public Slider sfxVolume;
    public Button saveButton;

    void OnEnable()
    {
        saveButton.onClick.AddListener(delegate { saveSettings(); });

        loadSettings();
    }

public void saveSettings()
    {
        JSONObject settingJSON = new JSONObject();

        settingJSON.Add("fullscreen", fullscreenToggle.isOn);
        settingJSON.Add("masterVolume", masterVolume.value);
        settingJSON.Add("musicVolume", musicVolume.value);
        settingJSON.Add("sfxVolume", sfxVolume.value);

        string path = Application.dataPath + "/Resources/gamesettings.json";
        File.WriteAllText(path, settingJSON.ToString());

        audioManager.loadSettings();

        MenuController.instance.closeOptions();
    }

    public void loadSettings()
    {
        pathToSettingJson = Application.dataPath + "/Resources/gamesettings.json";
        settings = JSON.Parse(File.ReadAllText(pathToSettingJson));

        fullscreenToggle.isOn = settings["fullscreen"];
        masterVolume.value = settings["masterVolume"];
        musicVolume.value = settings["musicVolume"];
        sfxVolume.value = settings["sfxVolume"];
    }
}