using System.Collections;
using UnityEngine;

public class FriendlyController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Transform target;
    public float mass = 1f;
    public float acceleration = 2f;
    public float linearDamp = 0f;
    private float moveForce;
    public float maxHealth = 100f;
    public float currentHealth;
    public float currentDamageRecived;
    public float damage = 10;
    public bool isHasHit;
    public bool isFind;
    public bool isClash;
    public bool isMoving;
    private Coroutine onFriendlyDamageCoroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentDamageRecived = 0f;
        rb.mass = mass;
        rb.linearDamping = linearDamp;
        moveForce = rb.mass * acceleration;

        currentHealth = maxHealth;
    }
    void Update()
    {
        if(isHasHit == false && isClash == false)
        {
            isMoving = true;
            GoToTarget();
        }
        else
        {
            isMoving = false;
        }
    }

    void GoToTarget()
    {
        target = FindClosest();

        if(target != null && isHasHit == false && isClash == false && isMoving == true)
        {
            Vector2 dir = (target.position - transform.position).normalized;
            rb.linearVelocity = dir * moveForce;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

    }
    public void OnFriendlyHit(float damage , Vector2 dir)
    {
        if(onFriendlyDamageCoroutine != null)
        {
            StopCoroutine(onFriendlyDamageCoroutine);
        }
                
        onFriendlyDamageCoroutine = StartCoroutine(FriendlyTakeDamage(damage,dir));
    }

    IEnumerator FriendlyTakeDamage(float damage , Vector2 dir)
    {

        currentDamageRecived += damage;
        currentHealth-= damage;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(-dir * moveForce * 2,ForceMode2D.Impulse);

        if(currentDamageRecived >= maxHealth)
        {
            Destroy(gameObject);
        }

        yield return new WaitForSeconds(0.5f);

        rb.linearVelocity = Vector2.zero;

        isHasHit = false; 
    }

    private Transform FindClosest()
    {
        float minDistance = 10;
        Transform closest = null;

        foreach (var friendly in EnemyManager.enemies)
        {
            if(friendly == null) continue;

            float distance = Vector2.Distance(gameObject.transform.position, friendly.transform.position);

            if(distance < minDistance)
            {
                isFind = true;
                minDistance = distance;
                closest = friendly.transform;
            }
        }
        return closest;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out EnemyController enemyController))
        {
            isClash = true;
            isHasHit = true;

            var dir = gameObject.transform.position - enemyController.transform.position;
            dir.Normalize();

            enemyController.isHasHit = true;
            enemyController.OnEnemyHit(damage, dir);
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        
    }
    
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            isClash = false;
        }
        if(collision.gameObject.CompareTag("Arrow"))
        {
            isHasHit = true;
        }
    }
}
