using UnityEngine;
using UnityEngine.UI;
using System.IO;
using SimpleJSON;
using System.Threading;
using System.Globalization;
using TMPro;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    static JSONNode settings;

    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public TMP_Dropdown qualityDropDown;
    public Slider masterVolume;
    public Slider musicVolume;
    public Slider effectsVolume;

    public AudioMixer audioMixer;

    public GameObject mainMenu;
    public GameObject settingsMenu;

    private List<Resolution> resolutions;

    private static string persistendFilePath;

    void OnEnable()
    {
        persistendFilePath = Application.persistentDataPath + "\\gamesettings.txt";
        resolutions = getResolutions();
        Invoke("loadSettingsInUI", 0f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            saveSettings();
            settingsMenu.SetActive(false);
            mainMenu.SetActive(true);
        }
    }

    public void saveSettings()
    {
        settings = getSettings();

        settings["resolution"] = resolutionDropdown.value;
        settings["fullscreen"] = fullscreenToggle.isOn;
        settings["quality"] = qualityDropDown.value;
        settings["masterVolume"] = masterVolume.value;
        settings["musicVolume"] = musicVolume.value;
        settings["effectsVolume"] = effectsVolume.value;

        File.WriteAllText(persistendFilePath, settings.ToString());
    }

    public void loadSettingsInUI()
    {
        settings = getSettings();

        fullscreenToggle.isOn = settings["fullscreen"];
        qualityDropDown.value = settings["quality"];
        masterVolume.value = settings["masterVolume"];
        musicVolume.value = settings["musicVolume"];
        effectsVolume.value = settings["effectsVolume"];


        //Load Resoltuions and remove Refresh Rate for Dropdown
        int currentResolutionIndex = 1;
        List<String> resolutionOptions = new List<string>();
        foreach (Resolution res in resolutions)
        {
            string option = res.width + " x " + res.height;
            resolutionOptions.Add(option);

            if (res.width == Screen.currentResolution.width && res.height == Screen.currentResolution.height)
                currentResolutionIndex = resolutions.IndexOf(res);
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(resolutionOptions);

        //If resolution not set in gamesettings load current resolution
        if (settings["resolution"] == -1)
            resolutionDropdown.value = currentResolutionIndex;
        else
            resolutionDropdown.value = settings["resolution"];
        resolutionDropdown.RefreshShownValue();
    }

    public static JSONNode getSettings()
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
        return settings;
    }

    public static List<Resolution> getResolutions()
    {
        Resolution[] resolution = Screen.resolutions;
        List<Resolution> res = new List<Resolution>();

        for (int i = 0; i < resolution.Length; i++)
        {
            if ((i + 1) < resolution.Length)
            {
                if (resolution[i].width != resolution[i + 1].width || resolution[i].height != resolution[i + 1].height)
                {
                    res.Add(resolution[i]);
                }
            } else
                res.Add(resolution[i]);
        }
        return res;
    }

    //Setting of Settings
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("Master", volume);
    }
    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("Music", volume);
    }

    public void SetEffectsVolume(float volume)
    {
        audioMixer.SetFloat("Effects", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen, resolution.refreshRate);
    }

}
