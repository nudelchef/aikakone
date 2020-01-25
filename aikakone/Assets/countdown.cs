using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using TMPro;

public class countdown : MonoBehaviour
{
    public int timeLeft = 60; //Seconds Overall
    public TextMeshProUGUI textMesh; //UI Text Object

    void Start()
    {
        //textMesh = gameObject.GetComponent<TextMeshProUGUI>();
        StartCoroutine("LoseTime");
        Time.timeScale = 1; //Just making sure that the timeScale is right
    }
    void Update()
    {
        string minutes = Mathf.Floor(timeLeft / 60).ToString("00");
        string seconds = (timeLeft % 60).ToString("00");
        textMesh.text = (minutes+":"+seconds); //Showing the Score on the Canvas
    }
    //Simple Coroutine
    IEnumerator LoseTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            timeLeft--;
        }
    }
}
