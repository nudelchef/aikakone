using UnityEngine;

public class audioManager : MonoBehaviour
{
    public static GameObject camera;
    void Start()
    {
        camera = GameObject.Find("Camera");
        playMusicOnObject(Resources.Load<AudioClip>("audio/music/1"), camera, true);
    }

    public static void playClipOnObject(AudioClip clip, GameObject objectToPlayOn, float volume = 0.25f)
    {
        AudioSource source = objectToPlayOn.GetComponent<AudioSource>();
        source.volume = volume; //set clip volume
        source.PlayOneShot(clip, 1f); //Play Clip
    }

    public static void playMusicOnObject(AudioClip clip, GameObject objectToPlayOn, bool loop = false, float volume = 0.15f)
    {
        AudioSource source = objectToPlayOn.GetComponent<AudioSource>();
        source.clip = clip; //Load Audio Clip
        source.volume = volume; //set clip volume
        source.loop = loop; //Loop clip
        source.Play(); //Play Clip
    }
}
