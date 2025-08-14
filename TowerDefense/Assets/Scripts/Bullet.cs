using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Attributes")]
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private bool isAreaDamage = false;
    [SerializeField] private float explosionRadius = 0.5f;

    private int bulletDamage = 1;
    private float freezeDuration = 0f;

    private Transform target;
    private bool hasHit = false;

    public void SetFreeze(float duration)
    {
        freezeDuration = duration;
    }

    public void SetTarget(Transform _target)
    {
        target = _target;
    }

    public void SetDamage(int damage)
    {
        bulletDamage = damage;
    }

    public void EnableAreaDamage(float radius)
    {
        isAreaDamage = true;
        explosionRadius = radius;
    }

    private void FixedUpdate()
    {
        if (!target) return;

        Vector2 direction = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        rb.velocity = direction * bulletSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasHit) return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (isAreaDamage)
            {
                Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
                foreach (Collider2D hit in hits)
                {
                    if (hit.CompareTag("Enemy"))
                    {
                        Health health = hit.GetComponent<Health>();
                        if (health != null)
                        {
                            health.TakeDamage(bulletDamage);
                        }

                        if (freezeDuration > 0f)
                        {
                            EnemyMovement enemyMovement = hit.GetComponent<EnemyMovement>();
                            if (enemyMovement != null)
                            {
                                enemyMovement.Freeze(freezeDuration, 0.5f); // 50% slow
                            }
                        }
                    }
                }
            }
            else
            {
                Health health = collision.gameObject.GetComponent<Health>();
                if (health != null)
                {
                    health.TakeDamage(bulletDamage);
                }

                if (freezeDuration > 0f)
                {
                    EnemyMovement enemyMovement = collision.gameObject.GetComponent<EnemyMovement>();
                    if (enemyMovement != null)
                    {
                        enemyMovement.Freeze(freezeDuration, 0.5f); // 50% slow
                    }
                }
            }

            hasHit = true;
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        if (isAreaDamage)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }

    }
    private void OnDrawGizmosSelected()
    {
        if (isAreaDamage)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}
