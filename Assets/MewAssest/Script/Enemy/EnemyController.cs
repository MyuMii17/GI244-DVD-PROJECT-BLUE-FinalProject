using System.Collections;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D cd;
    private Transform target;
    private LayerMask objectLayer;
    public float mass = 1f;
    public float acceleration = 2f;
    public float linearDamp = 0f;
    public float circleRange;
    private float moveForce;
    public float maxHealth = 100f;
    public float currentHealth;
    public float currentDamageRecived;
    public float damage = 10;
    public bool isHasHit;
    public bool isClash;
    public bool isMoving;
    public bool isFriendlyHit;
    public bool isPlayerHit;
    public bool isLongRange;
    private Coroutine onEnemyDamageCoroutine;
    private SpawnManager spawnManager;
    void Start()
    {
        spawnManager = SpawnManager.GetStatic();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<Collider2D>();
        objectLayer = LayerMask.GetMask("Object");

        currentDamageRecived = 0f;
        rb.mass = mass;
        rb.linearDamping = linearDamp;
        moveForce = rb.mass * acceleration;
        currentHealth = maxHealth;
        target = null;
    }
    void Update()
    {
        if(isHasHit == false  && isClash == false)
        {
            isMoving = true;
            GoToTarget();
        }
        else
        {
            isMoving = false;
        }

        if(PlayerManager.players.Count == 0 && isPlayerHit == true)
        {
            isPlayerHit = false;
        }
    }

    void GoToTarget()
    {
        FindCloset();

        if(target == null)
        {
            float minDistance = Mathf.Infinity;
            Transform detected = null;
            foreach (var objects in ObjectManager.objects) // หาสิ่งก่อสร้าง
            {
                if(objects == null) continue;

                float distance = Vector2.Distance(gameObject.transform.position, objects.transform.position);

                if(distance < minDistance)
                {
                    minDistance = distance;
                    detected = objects.transform;
                }
            }
            target = detected;
        }

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
    public void OnEnemyHit(float damage , Vector2 dir, Transform transform)
    {
        if(onEnemyDamageCoroutine != null)
        {
            StopCoroutine(onEnemyDamageCoroutine);
        }
        onEnemyDamageCoroutine = StartCoroutine(EnemyTakeDamage(damage,dir,transform));
    }

    IEnumerator EnemyTakeDamage(float damage , Vector2 dir, Transform transform)
    {

        currentDamageRecived += damage;
        currentHealth -= damage;
        target = transform;
        
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(-dir * moveForce * 2,ForceMode2D.Impulse);

        if(currentDamageRecived >= maxHealth)
        {
            spawnManager.enemiesDead++;
            Destroy(gameObject);
        }

        yield return new WaitForSeconds(0.5f);

        rb.linearVelocity = Vector2.zero;

        isHasHit = false;
    }

    public void FindCloset()
    {
        float minDistance = circleRange;
        Transform closest = null;
        Collider2D[] founds = Physics2D.OverlapCircleAll(transform.position, circleRange, objectLayer);
        foreach(Collider2D found in founds)
        {
            if(found == null) continue;

                float distance = Vector2.Distance(gameObject.transform.position, found.transform.position);

                if(distance < minDistance)
                {
                    minDistance = distance;
                    closest = found.transform;
                }
        }
        if(closest != null)
        {
            target = closest;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, circleRange);
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out FriendlyController friendlyController))    
        {
            isClash = true;
            isHasHit = true; 

            var dir = gameObject.transform.position - friendlyController.transform.position;
            dir.Normalize();

            friendlyController.OnFriendlyHit(damage, dir);
        }

        if (collision.gameObject.TryGetComponent(out PlayerController playerController))    
        {
            isClash = true;
            isHasHit = true; 

            var dir = gameObject.transform.position - playerController.transform.position;
            dir.Normalize();
            
            playerController.OnPlayerHit(damage, dir);
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Friendly"))    
        {
            isClash = false;;
            if (collision.gameObject.TryGetComponent(out FriendlyController friendlyController))
            {
                if(friendlyController.currentDamageRecived >= friendlyController.maxHealth)
                {
                    isFriendlyHit = false;
                }
            }
        }
        if (collision.gameObject.CompareTag("Player"))    
        {
            isClash = false;;
        }
    }
}
