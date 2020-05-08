using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class userInterface : MonoBehaviour
{

    public static int highscore = 0;
    public GameObject player;
    public PlayerHealth playerHealth;
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    private int lastHealth;
    public TextMeshProUGUI highscoreText; //UI Text Object

    bool gameHasEnded = false;
    public GameObject gameOverScreen;
    public TextMeshProUGUI gameOverScreenHighscoreText;
    public TextMeshProUGUI gameOverScreenEnemiesDefeatedText;
    public static int enemiesDefeated = 0;

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

    public void GameOver(bool hasWon = false)
    {
        // If the game has already ended, don't execute the code again
        if (gameHasEnded)
        {
            return;
        }

        gameOverScreenHighscoreText.text = "Highscore: " + userInterface.highscore.ToString();
        gameOverScreenEnemiesDefeatedText.text = "Enemies Defeated: " + enemiesDefeated.ToString();

        if (hasWon)
        {
            gameOverScreen.SetActive(true);
            Time.timeScale = 0f;
            Debug.Log("You Won!");
        }
        else
        {
            gameOverScreen.SetActive(true);
            Time.timeScale = 0f;
            Debug.Log("Game Over");
        }

        gameHasEnded = true;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}
