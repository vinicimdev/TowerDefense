using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private int hitPoints = 2;
    [SerializeField] private int goldWorth = 50;

    private bool isDestroyed = false;

    // Causa dano ao inimigo
    public void TakeDamage(int dmg)
    {
        hitPoints -= dmg;

        if(hitPoints <= 0 && !isDestroyed)
        {
            EnemySpawner.onEnemyDestroy.Invoke();
            LevelManager.main.IncreaseGold(goldWorth);
            isDestroyed = true;
            Destroy(gameObject);
        }
    }
}
