using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using SimpleJSON;
using System.Threading;
using System.Globalization;
using static audioManager;

public class SettingsController : MonoBehaviour {

    JSONNode settings;

    public Toggle fullscreenToggle;
    public Slider masterVolume;
    public Slider musicVolume;
    public Slider sfxVolume;
    public Button saveButton;
    private string persistendFilePath = Application.persistentDataPath + "\\gamesettings.txt";

    void OnEnable()
    {
        saveButton.onClick.AddListener(delegate { saveSettings(); });

        loadSettings();
    }

public void saveSettings()
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US"); // WRITE EVERYTHING AFTER THIS LINE!
        JSONObject settingJSON = new JSONObject();

        settingJSON.Add("fullscreen", fullscreenToggle.isOn);
        settingJSON.Add("masterVolume", masterVolume.value);
        settingJSON.Add("musicVolume", musicVolume.value);
        settingJSON.Add("sfxVolume", sfxVolume.value);

        File.WriteAllText(persistendFilePath, settingJSON.ToString());

        MenuController.instance.closeOptions();
    }

    public void loadSettings()
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US"); // WRITE EVERYTHING AFTER THIS LINE!
        if (System.IO.File.Exists(persistendFilePath))
        {
            settings = JSON.Parse(string.Join("", System.IO.File.ReadAllLines(persistendFilePath)));
        }
        else
        {
            settings = JSON.Parse((Resources.Load("gamesettings") as TextAsset).text);
        }

        fullscreenToggle.isOn = settings["fullscreen"];
        masterVolume.value = settings["masterVolume"];
        musicVolume.value = settings["musicVolume"];
        sfxVolume.value = settings["sfxVolume"];
    }
}