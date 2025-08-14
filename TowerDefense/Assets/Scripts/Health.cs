using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType { Normal, Tank }

public class Health : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] public int hitPoints = 2;
    [SerializeField] public int maxHitPoints;
    [SerializeField] public int goldWorth = 50;

    private bool isDestroyed = false;

    public EnemyType type = EnemyType.Normal;

    private void Awake()
    {
        maxHitPoints = hitPoints;
    }

    public void Initialize(int newHealth, int reward)
    {
        maxHitPoints = newHealth;
        hitPoints = newHealth;
        goldWorth = reward;
    }

    public int GetMaxHP()
    {
        return maxHitPoints;
    }

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
