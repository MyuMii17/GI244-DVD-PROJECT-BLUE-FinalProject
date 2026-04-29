using System.Collections.Generic;
using UnityEngine;


public class ArrowRicochet : MonoBehaviour
{
    public float mass = 1f;
    public float acceleration = 5f;
    public float linearDamp = 0f;
    public float arrowDamage = 5f;
    private int maxChain = 3;
    private int chainRange = 20;
    private int currentChain = 0;
    private float arrowForce;
    private List<Transform> hitTargets = new List<Transform>();
    private Rigidbody2D rb;
    private Transform target;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.mass = mass;
        rb.linearDamping = linearDamp;
        arrowForce = rb.mass * acceleration;
        currentChain = 0;
        hitTargets = new List<Transform>();
    }

    [System.Obsolete]
    void FixedUpdate()
    {
        if(target != null)
        {
            Vector2 dir = (target.position - transform.position).normalized;
            rb.velocity = dir * arrowForce;
        }
        else
        {
            rb.AddForce(transform.right * arrowForce);
        }
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        Transform enemy = other.transform;
        hitTargets.Add(enemy);

        EnemyController enemyController = other.GetComponent<EnemyController>();
            if(enemyController != null)
            {
                var dir = transform.position - enemyController.transform.position;
                dir.Normalize();

                enemyController.isHasHit = true;
                enemyController.OnEnemyHit(arrowDamage,dir);
            }

        Transform next = FindNextEnemy(other.transform);

        currentChain++;

        if (currentChain >= maxChain || next == null)
        {
            Destroy(gameObject);
            return;
        }

        if (next != null)
        {
            target = next;
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    private Transform FindNextEnemy(Transform current)
    {
        float minDistance = Mathf.Infinity;
        Transform next = null;

        foreach (var enemy in EnemyManager.enemies)
        {
            if(enemy == null) continue;
            if(enemy == current) continue;
            if(hitTargets.Contains(enemy)) continue;

            float distance = Vector2.Distance(gameObject.transform.position, enemy.transform.position);

            if(distance > chainRange) continue;

            if(distance < minDistance)
            {
                minDistance = distance;
                next = enemy;
            }
        }
        return next;
    }
}
