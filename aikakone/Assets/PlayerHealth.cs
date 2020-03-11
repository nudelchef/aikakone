using UnityEngine;
using static countdown;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;
    public GameObject player;
    public PlayerMovement playerMovement;

    private PlayerHealth()
    {
        //set current health to max on start
        currentHealth = maxHealth;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //pick up health
        //TODO: change wall1 to another object
        if(collision.gameObject.name == "wall1" && currentHealth != maxHealth)
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
        currentHealth = 2;
        Debug.Log(currentHealth);
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
        if (currentHealth <= 0 || countdown.timeLeft <= 0 )
        {
            killPlayer();
        }
    }
    public void killPlayer()
    {
        /*Destroy(player);

        Time.timeScale = 0;*///TODO
    }
}
