using SimpleJSON;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField nameField;
    [SerializeField]
    private Button playButton;

    private JSONNode settings;
    private string persistendFilePath;

    void OnEnable()
    {
        persistendFilePath = Application.persistentDataPath + "\\gamesettings.txt";
        getName();
    }

    public void Playgame()
    {
        saveName();
        SceneManager.LoadScene("LoadingScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void NameField()
    {
        nameField.text = nameField.text.Replace(" ", "");
        playButton.interactable = nameField.textComponent.text.Length > 1;
    }

    public void saveName()
    {
        PlayerHealth.name = nameField.text;
        settings = SettingsManager.getSettings();
        settings["name"] = nameField.text;

        File.WriteAllText(persistendFilePath, settings.ToString());
    }

    public void getName()
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US"); // WRITE EVERYTHING AFTER THIS LINE!
        if (System.IO.File.Exists(persistendFilePath))
        {
            settings = JSON.Parse(string.Join("", System.IO.File.ReadAllLines(persistendFilePath)));
            nameField.text = settings["name"];
        }

        NameField();
    }
}