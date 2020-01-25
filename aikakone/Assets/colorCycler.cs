using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colorCycler : MonoBehaviour
{
    public Color color1;
    public Color color2;
    public float duration = 3.0F;

    public Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
    }

    void Update()
    {
        float t = Mathf.PingPong(Time.time, duration) / duration;
        cam.backgroundColor = Color.Lerp(color1, color2, t);
    }
}
