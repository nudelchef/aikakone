using UnityEngine;
using System;
using System.IO;
using static countdown;
using static userInterface;
using static enemy;
using SimpleJSON;
using System.Threading;
using System.Globalization;
using System.Net;

//NEW
public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;
    public GameObject player;

    //vars for Highscore-JSON
    public static string name = "";

    private void OnCollisionEnter(Collision collision)
    {
        //pick up health
        //TODO: change wall1 to another object
        if (collision.gameObject.name == "wall1" && currentHealth != maxHealth)
        {
            Debug.Log(currentHealth);
            Destroy(collision.gameObject);
            currentHealth++;
            Update();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US"); // WRITE EVERYTHING AFTER THIS LINE!
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {

        //health can't be more than max
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        //death of the player
        if (currentHealth <= 0 || countdown.timeLeft <= 0)
        {
            killPlayer();
        }
    }
    public void killPlayer()
    {
        /*Destroy(player);

        Time.timeScale = 0;*///TODO
    }

    public void saveHighscore()     //by Sutorei
    {
        JSONNode oldHighscoreJSON;
        string persistendFilePath = Application.persistentDataPath + "\\highscores.txt";
        if (System.IO.File.Exists(persistendFilePath))
        {
            oldHighscoreJSON = JSON.Parse(string.Join("", System.IO.File.ReadAllLines(persistendFilePath)));
        }
        else
        {
            oldHighscoreJSON = JSON.Parse((Resources.Load("highscores") as TextAsset).text);
        }

        JSONObject highscoreJSON = new JSONObject();
        int highscore = userInterface.highscore;
        string currentTime = DateTime.Now.ToString();

        highscoreJSON.Add("Highscore:", highscore.ToString());
        highscoreJSON.Add("Name:", name); // TODO NICO: BRAUCHT EINE NAMENSEINGABE + UI-ANBINDUNG
        highscoreJSON.Add("Date:", currentTime); //Datum + Uhrzeit

        oldHighscoreJSON.Add(highscoreJSON);

        File.WriteAllText(persistendFilePath, oldHighscoreJSON.ToString());

        uploadHighscore(name, highscore, currentTime);
    }

    public void uploadHighscore(string name, int score, string timestamp)
    {
        string url = "http://h2882072.stratoserver.net/setHighscore.php" +
                     "?name=" + name +
                     "&score=" + score +
                     "&date=" + timestamp;
        try
        {
            using (var webClient = new WebClient())
            {
                var response = webClient.DownloadString(url);
            }
        }
        catch
        {
            //ERROR UPLOADING HIGHSCORE
        }

    }
}