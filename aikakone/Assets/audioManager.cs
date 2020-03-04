using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioManager : MonoBehaviour
{
    public static float sfxVolume = 0.15f;
    public static float musicVolume = 0.10f;
    public static GameObject spieler;
    public static GameObject camera;
    // Start is called before the first frame update
    void Start()
    {
        camera = GameObject.Find("Camera");
        playMusicOnObject(Resources.Load<AudioClip>("audio/music/1"),camera, musicVolume, true);
    }

    public static void playClipOnObject(AudioClip clip, GameObject objectToPlayOn,float volume = -1f)
    {
        if (volume < 0)
        {
            volume = sfxVolume;
        }
        objectToPlayOn.GetComponent<AudioSource>().volume = volume; //set clip volume
        objectToPlayOn.GetComponent<AudioSource>().PlayOneShot(clip, 1f); //Play Clip
    }
    private static int j = 0;
    public static void playMusicOnObject(AudioClip clip, GameObject objectToPlayOn, float volume = -1f, bool loop = false)
    {
        if (volume < 0)
        {
            volume = musicVolume;
        }
        objectToPlayOn.GetComponent<AudioSource>().clip = clip; //Load Audio Clip
        objectToPlayOn.GetComponent<AudioSource>().volume = volume; //set clip volume
        objectToPlayOn.GetComponent<AudioSource>().loop = loop; //Loop clip
        objectToPlayOn.GetComponent<AudioSource>().Play(); //Play Clip
    }
}
