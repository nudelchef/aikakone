using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SimpleJSON;
using System.Threading;
using System.Globalization;

public class audioManager : MonoBehaviour
{
    private static float sfxVolume;
    private static float musicVolume;
    public static GameObject spieler;
    public static GameObject camera;
    // Start is called before the first frame update
    void Start()
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US"); // WRITE EVERYTHING AFTER THIS LINE!
        loadSettings();

        camera = GameObject.Find("Camera");
        playMusicOnObject(Resources.Load<AudioClip>("audio/music/1"),camera, musicVolume, true);
    }

    public static void loadSettings()
    {
        JSONNode settings;
        string persistendFilePath = Application.persistentDataPath + "\\gamesettings.txt";
        if (System.IO.File.Exists(persistendFilePath))
        {
            settings = JSON.Parse(string.Join("", System.IO.File.ReadAllLines(persistendFilePath)));
        }
        else
        {
            settings = JSON.Parse((Resources.Load("gamesettings") as TextAsset).text);
        }


        musicVolume = (settings["musicVolume"] / 100f) * (settings["masterVolume"] / 100f);
        sfxVolume = (settings["sfxVolume"] / 100f) * (settings["masterVolume"] / 100f);
    }

    public static void playClipOnObject(AudioClip clip, GameObject objectToPlayOn,float volume = -1f)
    {
        if (volume < 0)
        {
            volume = sfxVolume;
        }
        AudioSource source = objectToPlayOn.GetComponent<AudioSource>();
        source.volume = volume; //set clip volume
        source.PlayOneShot(clip, 1f); //Play Clip
    }
    private static int j = 0;
    public static void playMusicOnObject(AudioClip clip, GameObject objectToPlayOn, float volume = -1f, bool loop = false)
    {
        if (volume < 0)
        {
            volume = musicVolume;
        }
        AudioSource source = objectToPlayOn.GetComponent<AudioSource>();
        source.clip = clip; //Load Audio Clip
        source.volume = volume; //set clip volume
        source.loop = loop; //Loop clip
        source.Play(); //Play Clip
    }
}
