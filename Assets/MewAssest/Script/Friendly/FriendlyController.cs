using System.Collections;
using UnityEngine;

public class FriendlyController : MonoBehaviour
{
    private Collider2D cd;
    private Rigidbody2D rb;
    private Transform target;
    private float rangeDistance;
    public Transform shootPosition;
    public Transform shootRoataion;
    public GameObject arrowPrefeb;
    private LayerMask defaultLayer;
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
    public bool isFind;

    public bool isMoving;

    [Header("Range Friendly")]
    public float shootCooldown;
    private float nextShoot;

    public bool isLongRange;

    private Coroutine onFriendlyDamageCoroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<Collider2D>();
        defaultLayer = LayerMask.GetMask("Default");
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
        FindClosest();

        if (target != null)
        {
            rangeDistance = Vector2.Distance(gameObject.transform.position, target.transform.position);
        }

        if (isLongRange == true && isFind && rangeDistance <= 5)
        {
            if (target != null)
            {
                rb.linearVelocity = Vector2.zero;
                Shoot();
            }
        }
        else if(target != null && !isHasHit && !isClash && isMoving)
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
        cd.enabled = false;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(-dir * push * 2,ForceMode2D.Impulse);

        if(currentDamageRecived >= maxHealth)
        {
            
            currentHealth = maxHealth;
            currentDamageRecived = 0;
            // FriendliesPool.GetInstance().ReturnFriend(this.gameObject);
            Destroy(gameObject);
        }

        yield return new WaitForSeconds(1);

        rb.linearVelocity = Vector2.zero;
        cd.enabled = true;
        isHasHit = false; 
    }

    private void FindClosest()
    {
        if(ObjectManager.objects.Count == 0) return;

        float minDistance = circleRange;
        Transform closest = null;
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, circleRange);

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

        if(closest != null)
        {
            isFind = true;
            target = closest;
        }
        else
        {
            isFind = false;
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, circleRange);
    }

    void Shoot()
    {
        var targetPosition = target.transform.position;

        var dir = (targetPosition - transform.position).normalized;

        float angle = Mathf.Atan2(dir.y ,dir.x) * Mathf.Rad2Deg;

        shootRoataion.transform.rotation = Quaternion.Euler(0, 0, angle); 
        
        if (Time.time >= nextShoot)
        {
            var arrow = Instantiate(
                arrowPrefeb,
                shootPosition.position,
                shootPosition.rotation
            );
            Destroy(arrow, 2);
            nextShoot = Time.time + shootCooldown;
        }

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
