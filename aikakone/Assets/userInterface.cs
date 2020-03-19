using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class userInterface : MonoBehaviour
{

    public static int highscore;
    public GameObject player;
    public PlayerHealth playerHealth;
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    private int lastHealth;
    public TextMeshProUGUI highscoreText; //UI Text Object

    // Start is called before the first frame update
    void Start()
    {
        playerHealth = player.GetComponent<PlayerHealth>();
        lastHealth = playerHealth.currentHealth;
        fullHeart = Resources.Load<Sprite>("UiTextures/fullHeart");
        emptyHeart = Resources.Load<Sprite>("UiTextures/emptyHeart");
        updateHearts();
        highscore = 0;
        updateHighscore();
    }

    // Update is called once per frame
    void Update()
    {
        updateHighscore();

        if (lastHealth != playerHealth.currentHealth)
        {
            updateHearts();
        }
    }

    public void updateHighscore()
    {
        highscoreText.text = highscore.ToString();
    }

    private void updateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < playerHealth.currentHealth)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            //TODO max hearts not visible when increased
            if (i < playerHealth.maxHealth)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
        lastHealth = playerHealth.currentHealth;
    }
}
