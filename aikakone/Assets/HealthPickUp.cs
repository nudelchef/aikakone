using UnityEngine;

public class HealthPickUp : MonoBehaviour
{
    PlayerHealth playerHealth;

    public int healthBonus = 1;

    private void Awake()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (playerHealth.currentHealth < playerHealth.maxHealth)
        {
            Destroy(gameObject);
            playerHealth.currentHealth = playerHealth.currentHealth + healthBonus;
        }
    }
}
