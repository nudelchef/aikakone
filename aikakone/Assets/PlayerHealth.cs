using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections.Generic;
using static countdown;
using static userInterface;
using static enemy;
using SimpleJSON;
using System.Threading;
using System.Globalization;

//NEW
public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;
    public GameObject player;

    //vars for Highscore-JSON
    public string name = "Test";

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
        string persistendFilePath = Application.persistentDataPath+"\\highscores.txt";
        if (System.IO.File.Exists(persistendFilePath))
        {
            oldHighscoreJSON = JSON.Parse(string.Join("",System.IO.File.ReadAllLines(persistendFilePath)));
        }
        else
        {
            oldHighscoreJSON = JSON.Parse((Resources.Load("highscores") as TextAsset).text);
        }

        JSONObject highscoreJSON = new JSONObject();
        string highscore = userInterface.highscore.ToString();

        highscoreJSON.Add("Highscore:", highscore);
        highscoreJSON.Add("Name:", name); // TODO NICO: BRAUCHT EINE NAMENSEINGABE + UI-ANBINDUNG
        highscoreJSON.Add("Date:", DateTime.Now.ToString()); //Datum + Uhrzeit

        oldHighscoreJSON.Add(highscoreJSON);

        File.WriteAllText(persistendFilePath, oldHighscoreJSON.ToString());
    }
}