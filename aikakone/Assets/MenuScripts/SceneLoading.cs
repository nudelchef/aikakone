using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoading : MonoBehaviour
{
    [SerializeField]
    private Image _progressBar;

    void Start()
    {
        StartCoroutine(LoadAsynOperation());
    }

    IEnumerator LoadAsynOperation()
    {
        AsyncOperation gameLevel = SceneManager.LoadSceneAsync("InGame");

        while(gameLevel.progress < 1)
        {
            _progressBar.fillAmount = gameLevel.progress;
            yield return new WaitForEndOfFrame();
        }

    }
}
