using System.Collections;
using UnityEngine;

public class FriendlyController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Transform target;
    public Transform shootPosition;
    public Transform shootRoataion;
    public float mass = 1f;
    public float acceleration = 2f;
    public float linearDamp = 0f;
    public float pushForce;
    private float moveForce;
    public float maxHealth = 100f;
    public float currentHealth;
    public float currentDamageRecived;
    public float damage = 10;
    public bool isHasHit;
    public bool isClash;
    public float circleRange = 5;

    public bool isMoving;

    private Coroutine onFriendlyDamageCoroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        shootRoataion = transform.GetChild(0).transform;
        shootPosition = shootRoataion.transform.GetChild(0).transform.GetChild(0).transform;

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

            float angle = Mathf.Atan2(dir.y ,dir.x) * Mathf.Rad2Deg;
            shootRoataion.transform.rotation = Quaternion.Euler(0, 0, angle); 
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

    }
    public void OnFriendlyHit(float damage , Vector2 dir, float push)
    {
        if(onFriendlyDamageCoroutine != null)
        {
            StopCoroutine(onFriendlyDamageCoroutine);
        }
                
        onFriendlyDamageCoroutine = StartCoroutine(FriendlyTakeDamage(damage,dir,push));
    }

    IEnumerator FriendlyTakeDamage(float damage , Vector2 dir, float push)
    {

        currentDamageRecived += damage;
        currentHealth-= damage;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(-dir * push * 2,ForceMode2D.Impulse);

        if(currentDamageRecived >= maxHealth)
        {
            Destroy(gameObject);
        }

        yield return new WaitForSeconds(1);

        rb.linearVelocity = Vector2.zero;

        isHasHit = false; 
    }

    private Transform FindClosest()
    {
        float minDistance = circleRange;
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, circleRange);

        Transform closest = null;

        foreach (var enemy in enemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                if(enemy == null) continue;

                float distance = Vector2.Distance(gameObject.transform.position, enemy.transform.position);

                if(distance < minDistance)
                {
                    minDistance = distance;
                    closest = enemy.transform;
                }
            }
        }
        return closest;
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, circleRange);
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

            enemyController.OnEnemyHit(damage, dir, gameObject.transform, pushForce);
        }
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
