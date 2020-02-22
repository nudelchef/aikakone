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
        playClipOnObject(Resources.Load<AudioClip>("audio/music/1"),camera, musicVolume, true);
    }

    private static int j = 0;
    public static void playClipOnObject(AudioClip clip, GameObject objectToPlayOn,float volume = -1f, bool loop = false)
    {
        if (volume < 1)
        {
            volume = sfxVolume;
        }
        objectToPlayOn.GetComponents<AudioSource>()[j].clip = clip; //Load Audio Clip
        objectToPlayOn.GetComponents<AudioSource>()[j].Play(); //Play Clip
        objectToPlayOn.GetComponents<AudioSource>()[j].volume = volume; //set clip volume
        objectToPlayOn.GetComponents<AudioSource>()[j].loop = loop; //Loop clip
        j++;
        if (j > objectToPlayOn.GetComponents<AudioSource>().Length - 1)
        {
            j = 0;
        }
    }
}
