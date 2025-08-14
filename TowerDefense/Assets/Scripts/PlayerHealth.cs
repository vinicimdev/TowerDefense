using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private TextMeshProUGUI healthText;

    private void Awake()
    {
        UpdateHealthUI();
    }

    void TakeDamage(int damage)
    {
        health -= damage;
        UpdateHealthUI();

        if (health <= 0)
            RestartLevel();
    }
    private void UpdateHealthUI()
    {
        healthText.text = health.ToString();
    }

    void RestartLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            Health enemyHealth = collision.GetComponent<Health>();
            if (enemyHealth != null)
            {
                int dmg = enemyHealth.GetMaxHP();
                TakeDamage(dmg);
                Debug.Log($"Jogador sofreu {dmg} de dano! Vida restante: {health}");
            }
        }
    }
}
